using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Core;
using Nop.Services.Helpers;
using Nop.Services.Seo;
using Nop.Core.Domain.Catalog;
using Microsoft.AspNetCore.Hosting;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Models.Catalog;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Kendoui;
using System.IO;
using Nop.Web.Models.Directory;
using Nop.Web.Framework.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nop.Web.Areas.Admin.Controllers
{
    public class DirectorysController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly CatalogSettings _catalogSettings;      
        private readonly IHostingEnvironment _hostingEnvironment;
       
        #endregion

        #region Constructors

        public DirectorysController(
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            IStoreContext storeContext,
            IPermissionService permissionService,
            IStoreService storeService,
            CatalogSettings catalogSettings,          
            IHostingEnvironment hostingEnvironment       )
        {
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            var model = new DirectoryListModel();
            //var models = new List<DriveInfoModel>();
            DriveInfo[] drives = DriveInfo.GetDrives();
			CommonHelper.UploadFilePath = drives[0].Name;
			// model.AvailableDrivers.Add(new SelectListItem { Text = ("Admin.Common.All"), Value = "0" });
            foreach (var item in drives)
                model.AvailableDrivers.Add(new SelectListItem { Text = item.Name, Value = item.Name });
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command, DirectoryListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedKendoGridJson();
			FileSystemInfo[] dirFileitems=null;
			var list = new List<DirectoryInfoModel>();
			if (string.IsNullOrEmpty(model.SearchDriverName))
			{
				DriveInfo[] drives = DriveInfo.GetDrives();  
				DirectoryInfo dirInfo = new DirectoryInfo(model.SearchDriverId);//根目录
				CommonHelper.UploadFilePath = model.SearchDriverId;
				dirFileitems = dirInfo.GetFileSystemInfos();
			}
			else
			{
				DirectoryInfo dirInfo = new DirectoryInfo(model.SearchDriverName);//根目录
				CommonHelper.UploadFilePath = model.SearchDriverName;
				dirFileitems = dirInfo.GetFileSystemInfos();
			}
			foreach (var item in dirFileitems)
			{
				if (item is DirectoryInfo)
				{
					var directory = item as DirectoryInfo;
					if ((directory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (directory.Attributes & FileAttributes.System) != FileAttributes.System)
					{
						list.Add(new DirectoryInfoModel()
						{
							Root = directory.Root,
							FullName = directory.FullName,
							IsDir = true,
                            Icon= "folder.png",
							Name = directory.Name,
							Parent = directory.Parent,
							CreationTime = directory.CreationTime,
							Exists = directory.Exists,
							Extension = directory.Extension,
							LastAccessTime = directory.LastAccessTime,
							LastWriteTime = directory.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
						});
					}
				}
				else if (item is FileInfo)
				{
					var file = item as FileInfo;
					if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (file.Attributes & FileAttributes.System) != FileAttributes.System)
					{
						list.Add(new DirectoryInfoModel()
						{

							FullName = file.FullName,
							// FullPath=file.FullPath
							IsDir = false,
							Name = file.Name,
							CreationTime = file.CreationTime,
							Exists = file.Exists,
							Extension = file.Extension,
							LastAccessTime = file.LastAccessTime,
							LastWriteTime = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
							IsReadOnly = file.IsReadOnly,
							Directory = file.Directory,
							DirectoryName = file.DirectoryName,
							Length = file.Length,
						});
					}

				}
			} 
			//var categories = new DirectoryListModel(); 
			var gridModel = new DataSourceResult
			{
				Data = list.OrderByDescending(o=>o.IsDir),
				Total = list.Count
			};
			return Json(gridModel);
		}

        public virtual IActionResult DirOrFileAdd(DirectoryInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView(); 

            return new NullJsonResult();
        }
        public virtual IActionResult DirOrFileUpdate(DirectoryInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView(); 
            return new NullJsonResult();
        }

        public virtual IActionResult DirOrFileDelete(DirectoryInfoModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();
            
            return new NullJsonResult();
        }

    }
}
