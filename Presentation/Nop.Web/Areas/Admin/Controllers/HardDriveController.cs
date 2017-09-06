using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Services.Security;
using Nop.Core;
using Microsoft.AspNetCore.Hosting;
using Nop.Core.Domain.Catalog;
using Nop.Services.Stores;
using Nop.Services.Customers;
using Nop.Services.Seo;
using System.IO;
using Nop.Web.Models.Directory;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class HardDriveController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper; 
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

 

        #region Constructors

        public HardDriveController(
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper, 
            IWorkContext workContext,
            IStoreContext storeContext,
            IPermissionService permissionService,
            IStoreService storeService,
            CatalogSettings catalogSettings,
            IHostingEnvironment hostingEnvironment)
        {
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper; 
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._catalogSettings = catalogSettings;
            this._hostingEnvironment = hostingEnvironment;
        }

        #endregion
        // GET: /<controller>/
        public IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            var model = new DriveInfoModel();
            return View(model);
        }
       [HttpPost]
        public virtual IActionResult DriveList(DataSourceRequest command, DriveInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView(); 
            DriveInfo[] drives = DriveInfo.GetDrives();
            var models = new List<DriveInfoModel>();
            int index = 0;
            foreach (var item in drives)
            {
                index++;
                var drive = new DriveInfoModel()
                {
                    Id = index,
                    AvailableFreeSpace = item.AvailableFreeSpace,
                    AvailableFreeSpaceText = ByteFormatter.ToString(item.AvailableFreeSpace) + " 可用",
                    DriveFormat = item.DriveFormat,
                    DriveType = item.DriveType,
                    IsReady = item.IsReady,
                    Name = item.Name,
                    NameDesc = string.Format("({0}:)", item.ToString().Replace(":", "").Replace("\\", "")),
                    RootDirectory = item.RootDirectory,
                    TotalFreeSpace = item.TotalFreeSpace,
                    TotalFreeSpaceText = ByteFormatter.ToString(item.TotalFreeSpace),
                    TotalSize = item.TotalSize,
                    TotalSizeText = "共" + ByteFormatter.ToString(item.TotalSize),
                    VolumeLabel = string.IsNullOrEmpty(item.VolumeLabel) ? "本地磁盘 " : item.VolumeLabel,
                    Percent = 100 - (int)(item.AvailableFreeSpace * 100.0 / item.TotalSize),
                    DriveLetter = item.Name.Replace("\\", "")

                };
                models.Add(drive);
            }
            var gridModel = new DataSourceResult
            {
                Data =models,
                Total = models.Count
            };
            return Json(gridModel);
           
        }
       
        public virtual IActionResult FormatDrive(DriveInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
           // CommonHelper.FormatDrive(model.DriveLetter);
            return new NullJsonResult();
        }
    }
}
