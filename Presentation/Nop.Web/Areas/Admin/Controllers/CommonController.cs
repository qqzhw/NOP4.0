using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Security;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class CommonController : BaseAdminController
    {
        #region Fields

       
        private readonly ICustomerService _customerService;
      
        private readonly IWebHelper _webHelper;       
     
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;     
        private readonly IStoreService _storeService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStaticCacheManager _cacheManager;

        #endregion

        #region Constructors

        public CommonController( 
            ICustomerService customerService, 
            IWebHelper webHelper,                 
            IWorkContext workContext,
            IStoreContext storeContext,
            IPermissionService permissionService,        
            IStoreService storeService,
            CatalogSettings catalogSettings,
            IMaintenanceService maintenanceService,
            IHostingEnvironment hostingEnvironment,
            IStaticCacheManager cacheManager)
        {         
            this._customerService = customerService; 
            this._webHelper = webHelper;          
         
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._permissionService = permissionService;          
            this._storeService = storeService;
            this._catalogSettings = catalogSettings;
            this._maintenanceService = maintenanceService;
            this._hostingEnvironment = hostingEnvironment;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region Utilities

        private bool IsDebugAssembly(Assembly assembly)
        {
            var attribs = assembly.GetCustomAttributes(typeof(System.Diagnostics.DebuggableAttribute), false);

            if (attribs.Length > 0)
            {
                var attr = attribs[0] as System.Diagnostics.DebuggableAttribute;
                if (attr != null)
                {
                    return attr.IsJITOptimizerDisabled;
                }
            }

            return false;
        }

        private DateTime GetBuildDate(Assembly assembly, TimeZoneInfo target = null)
        {
            var filePath = assembly.Location;

            const int cPeHeaderOffset = 60;
            const int cLinkerTimestampOffset = 8;

            var buffer = new byte[2048];

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                stream.Read(buffer, 0, 2048);
            }

            var offset = BitConverter.ToInt32(buffer, cPeHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, offset + cLinkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var linkTimeUtc = epoch.AddSeconds(secondsSince1970);

            var tz = target ?? TimeZoneInfo.Local;
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(linkTimeUtc, tz);

            return localTime;
        }

        #endregion

        #region Methods

        public virtual IActionResult SystemInfo()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new SystemInfoModel();
            model.NopVersion = NopVersion.CurrentVersion;
            try
            {
                model.OperatingSystem = Environment.OSVersion.VersionString;
            }
            catch (Exception) { }
            try
            {
                model.AspNetInfo = RuntimeEnvironment.GetSystemVersion();
            }
            catch (Exception) { }
            try
            {
                model.IsFullTrust = AppDomain.CurrentDomain.IsFullyTrusted.ToString();
            }
            catch (Exception) { }
            model.ServerTimeZone = TimeZone.CurrentTimeZone.StandardName;
            model.ServerLocalTime = DateTime.Now;
            model.UtcTime = DateTime.UtcNow;
            model.CurrentUserTime = (DateTime.Now);
            model.HttpHost = HttpContext.Request.Headers[HeaderNames.Host];

            foreach (var header in HttpContext.Request.Headers)
            {
                model.Headers.Add(new SystemInfoModel.HeaderModel
                {
                    Name = header.Key,
                    Value = header.Value
                });
            }

            var trustLevel = CommonHelper.GetTrustLevel();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var loadedAssembly = new SystemInfoModel.LoadedAssembly
                {
                    FullName = assembly.FullName,

                };
                //ensure no exception is thrown
                try
                {
                    var canGetLocation = trustLevel >= AspNetHostingPermissionLevel.High && !assembly.IsDynamic;
                    loadedAssembly.Location = canGetLocation ? assembly.Location : null;
                    loadedAssembly.IsDebug = IsDebugAssembly(assembly);
                    loadedAssembly.BuildDate = canGetLocation ? (DateTime?)GetBuildDate(assembly, TimeZoneInfo.Local) : null;
                }
                catch (Exception) { }
                model.LoadedAssemblies.Add(loadedAssembly);
            }

            return View(model);
        }

        public virtual IActionResult Warnings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new List<SystemWarningModel>();

            //store URL
            var currentStoreUrl = _storeContext.CurrentStore.Url;
            if (!String.IsNullOrEmpty(currentStoreUrl) &&
                (currentStoreUrl.Equals(_webHelper.GetStoreLocation(false), StringComparison.InvariantCultureIgnoreCase)
                ||
                currentStoreUrl.Equals(_webHelper.GetStoreLocation(true), StringComparison.InvariantCultureIgnoreCase)
                ))
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = ("Admin.System.Warnings.URL.Match")
                });
            else
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = string.Format(("Admin.System.Warnings.URL.NoMatch"), currentStoreUrl, _webHelper.GetStoreLocation(false))
                });           
                     
            //incompatible plugins
            if (PluginManager.IncompatiblePlugins != null)
                foreach (var pluginName in PluginManager.IncompatiblePlugins)
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(("Admin.System.Warnings.IncompatiblePlugin"), pluginName)
                    });

            //performance settings
            if (!_catalogSettings.IgnoreStoreLimitations && _storeService.GetAllStores().Count == 1)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = ("Admin.System.Warnings.Performance.IgnoreStoreLimitations")
                });
            }
            if (!_catalogSettings.IgnoreAcl)
            {
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Warning,
                    Text = ("Admin.System.Warnings.Performance.IgnoreAcl")
                });
            }

            //validate write permissions (the same procedure like during installation)
            var dirPermissionsOk = true;
            var dirsToCheck = FilePermissionHelper.GetDirectoriesWrite();
            foreach (string dir in dirsToCheck)
                if (!FilePermissionHelper.CheckPermissions(dir, false, true, true, false))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(("Admin.System.Warnings.DirectoryPermission.Wrong"), WindowsIdentity.GetCurrent().Name, dir)
                    });
                    dirPermissionsOk = false;
                }
            if (dirPermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = ("Admin.System.Warnings.DirectoryPermission.OK")
                });

            var filePermissionsOk = true;
            var filesToCheck = FilePermissionHelper.GetFilesWrite();
            foreach (string file in filesToCheck)
                if (!FilePermissionHelper.CheckPermissions(file, false, true, true, true))
                {
                    model.Add(new SystemWarningModel
                    {
                        Level = SystemWarningLevel.Warning,
                        Text = string.Format(("Admin.System.Warnings.FilePermission.Wrong"), WindowsIdentity.GetCurrent().Name, file)
                    });
                    filePermissionsOk = false;
                }
            if (filePermissionsOk)
                model.Add(new SystemWarningModel
                {
                    Level = SystemWarningLevel.Pass,
                    Text = ("Admin.System.Warnings.FilePermission.OK")
                });

            return View(model);
        }

        public virtual IActionResult Maintenance()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new MaintenanceModel();
            model.DeleteGuests.EndDate = DateTime.UtcNow.AddDays(-7);
            model.DeleteGuests.OnlyWithoutShoppingCart = true;
            model.DeleteAbandonedCarts.OlderThan = DateTime.UtcNow.AddDays(-182);
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-guests")]
        public virtual IActionResult MaintenanceDeleteGuests(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            DateTime? startDateValue = (model.DeleteGuests.StartDate == null) ? null
                            : (DateTime?)(model.DeleteGuests.StartDate.Value);

            DateTime? endDateValue = (model.DeleteGuests.EndDate == null) ? null
                            : (DateTime?)model.DeleteGuests.EndDate.Value.AddDays(1);

            model.DeleteGuests.NumberOfDeletedCustomers = _customerService.DeleteGuestCustomers(startDateValue, endDateValue, model.DeleteGuests.OnlyWithoutShoppingCart);

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-abondoned-carts")]
        public virtual IActionResult MaintenanceDeleteAbandonedCarts(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var olderThanDateValue = (model.DeleteAbandonedCarts.OlderThan);

            model.DeleteAbandonedCarts.NumberOfDeletedItems = 0;
            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("delete-exported-files")]
        public virtual IActionResult MaintenanceDeleteFiles(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            DateTime? startDateValue = (model.DeleteExportedFiles.StartDate == null) ? null
                            : (DateTime?)(model.DeleteExportedFiles.StartDate.Value);

            DateTime? endDateValue = (model.DeleteExportedFiles.EndDate == null) ? null
                            : (DateTime?)(model.DeleteExportedFiles.EndDate.Value).AddDays(1);


            model.DeleteExportedFiles.NumberOfDeletedFiles = 0;
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "files\\exportimport");
            foreach (var fullPath in Directory.GetFiles(path))
            {
                try
                {
                    var fileName = Path.GetFileName(fullPath);
                    if (fileName.Equals("index.htm", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    var info = new FileInfo(fullPath);
                    if ((!startDateValue.HasValue || startDateValue.Value < info.CreationTimeUtc) &&
                        (!endDateValue.HasValue || info.CreationTimeUtc < endDateValue.Value))
                    {
                        System.IO.File.Delete(fullPath);
                        model.DeleteExportedFiles.NumberOfDeletedFiles++;
                    }
                }
                catch (Exception exc)
                {
                    ErrorNotification(exc, false);
                }
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult BackupFiles(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedKendoGridJson();

            var backupFiles = _maintenanceService.GetAllBackupFiles().ToList();

            var gridModel = new DataSourceResult
            {
                Data = backupFiles.Select(p => new {
                    p.Name,
                    Length = string.Format("{0:F2} Mb", p.Length / 1024f / 1024f),
                    Link = _webHelper.GetStoreLocation(false) + "db_backups/" + p.Name
                }),
                Total = backupFiles.Count
            };
            return Json(gridModel);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backup-database")]
        public virtual IActionResult BackupDatabase(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            try
            {
                _maintenanceService.BackupDatabase();
                this.SuccessNotification(("Admin.System.Maintenance.BackupDatabase.BackupCreated"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return View(model);
        }

        [HttpPost, ActionName("Maintenance")]
        [FormValueRequired("backupFileName", "action")]
        public virtual IActionResult BackupAction(MaintenanceModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var action = this.Request.Form["action"];

            var fileName = this.Request.Form["backupFileName"];
            var backupPath = _maintenanceService.GetBackupPath(fileName);

            try
            {
                switch (action)
                {
                    case "delete-backup":
                        {
                            System.IO.File.Delete(backupPath);
                            this.SuccessNotification(string.Format(("Admin.System.Maintenance.BackupDatabase.BackupDeleted"), fileName));
                        }
                        break;
                    case "restore-backup":
                        {
                            _maintenanceService.RestoreDatabase(backupPath);
                            this.SuccessNotification(("Admin.System.Maintenance.BackupDatabase.DatabaseRestored"));
                        }
                        break;
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return View(model);
        }

    
        [HttpPost]
        public virtual IActionResult ClearCache(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            _cacheManager.Clear();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

        [HttpPost]
        public virtual IActionResult RestartApplication(string returnUrl = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }


        public virtual IActionResult SeNames()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            var model = new UrlRecordListModel();
            return View(model);
        }
         [HttpPost]
        public virtual IActionResult DeleteSelectedSeNames(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMaintenance))
                return AccessDeniedView();

            

            return Json(new { Result = true });
        }
         
        #endregion
    }
}