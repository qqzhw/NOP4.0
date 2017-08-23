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

            return null;
            //var categories = new DirectoryListModel(); 
            //var gridModel = new DataSourceResult
            //{
            //    Data = categories.Select(x =>
            //    {
            //        var categoryModel = x.ToModel();
                 
            //        return categoryModel; 
            //    }),
            //    Total = categories.TotalCount
            //};
            //return Json(gridModel);
        }

    }
}
