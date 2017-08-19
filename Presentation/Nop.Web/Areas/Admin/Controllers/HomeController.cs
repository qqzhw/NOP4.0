using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Services.Directory;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class HomeController : BaseAdminController
    {
        #region Fields

        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IMachineInfoService _machineInfoService;
        #endregion

        #region Ctor

        public HomeController(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings, 
            ISettingService settingService,
            IWorkContext workContext, IMachineInfoService  machineInfoService)
        {
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._workContext = workContext;
            _machineInfoService = machineInfoService;
        }
        
        #endregion
        
        #region Methods

        public virtual IActionResult Index()
        {
            _machineInfoService.GetCPUSerialNumber();
            var model = new DashboardModel(); 
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult NopCommerceNewsHideAdv()
        {
            _adminAreaSettings.HideAdvertisementsOnAdminArea = !_adminAreaSettings.HideAdvertisementsOnAdminArea;
            _settingService.SaveSetting(_adminAreaSettings);

            return Content("Setting changed");
        }

        #endregion
    }
}