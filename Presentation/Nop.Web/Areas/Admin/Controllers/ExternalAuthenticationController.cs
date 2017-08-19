using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ExternalAuthenticationController : BaseAdminController
	{
		#region Fields

        private readonly IExternalAuthenticationService _externalAuthenticationService;
       
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IPluginFinder _pluginFinder;

        #endregion

        #region Ctor

        public ExternalAuthenticationController(IExternalAuthenticationService externalAuthenticationService, 
            
            ISettingService settingService,
            IPermissionService permissionService,
            IPluginFinder pluginFinder)
		{
            this._externalAuthenticationService = externalAuthenticationService;
          
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._pluginFinder = pluginFinder;
		}

		#endregion 

        #region Methods

        public virtual IActionResult Methods()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult Methods(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedKendoGridJson();

            var methodsModel = new List<AuthenticationMethodModel>();
            var methods = _externalAuthenticationService.LoadAllExternalAuthenticationMethods();
            foreach (var method in methods)
            {
                var tmp1 = method.ToModel(); 
                tmp1.ConfigurationUrl = method.GetConfigurationPageUrl();
                methodsModel.Add(tmp1);
            }
            methodsModel = methodsModel.ToList();
            var gridModel = new DataSourceResult
            {
                Data = methodsModel,
                Total = methodsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult MethodUpdate(AuthenticationMethodModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var eam = _externalAuthenticationService.LoadExternalAuthenticationMethodBySystemName(model.SystemName);
         
           
            {
                if (model.IsActive)
                {
                   
                }
            }
            var pluginDescriptor = eam.PluginDescriptor;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;
            PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
            //reset plugin cache
            _pluginFinder.ReloadPlugins();

            return new NullJsonResult();
        }
        
        #endregion
    }
}