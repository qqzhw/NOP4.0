using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Plugins;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Customers;
 
using Nop.Core.Plugins;
using Nop.Services;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Customers; 
using Nop.Services.Logging; 
using Nop.Services.Security; 
using Nop.Services.Stores;
 
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PluginController : BaseAdminController
	{
		#region Fields

        private readonly IPluginFinder _pluginFinder;
        private readonly IOfficialFeedManager _officialFeedManager;
        
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        
	    private readonly ISettingService _settingService;
	    private readonly IStoreService _storeService;  
        private readonly WidgetSettings _widgetSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerService _customerService;
        
        #endregion

        #region Ctor

        public PluginController(IPluginFinder pluginFinder,
            IOfficialFeedManager officialFeedManager, 
            IWebHelper webHelper,
            IPermissionService permissionService,  
            ISettingService settingService, 
            IStoreService storeService, 
            WidgetSettings widgetSettings,
            ICustomerActivityService customerActivityService,
            ICustomerService customerService)
        {
            this._pluginFinder = pluginFinder;
            this._officialFeedManager = officialFeedManager;
         
            this._webHelper = webHelper;
            this._permissionService = permissionService;
        
            this._settingService = settingService;
            this._storeService = storeService; 
            
            this._widgetSettings = widgetSettings;
            this._customerActivityService = customerActivityService;
            this._customerService = customerService;
        }

		#endregion 

        #region Utilities

        protected virtual PluginModel PreparePluginModel(PluginDescriptor pluginDescriptor, 
            bool prepareLocales = true, bool prepareStores = true, bool prepareAcl = true)
        {
            var pluginModel = pluginDescriptor.ToModel();
            //logo
            pluginModel.LogoUrl = pluginDescriptor.GetLogoUrl(_webHelper);

            if (prepareLocales)
            {
               
            }
            if (prepareStores)
            {
                //stores
                pluginModel.SelectedStoreIds = pluginDescriptor.LimitedToStores;
                var allStores = _storeService.GetAllStores();
                foreach (var store in allStores)
                {
                    pluginModel.AvailableStores.Add(new SelectListItem
                    {
                        Text = store.Name,
                        Value = store.Id.ToString(),
                        Selected = pluginModel.SelectedStoreIds.Contains(store.Id)
                    });
                }
            }

            if (prepareAcl)
            {
                //acl
                pluginModel.SelectedCustomerRoleIds = pluginDescriptor.LimitedToCustomerRoles;
                foreach (var role in _customerService.GetAllCustomerRoles(true))
                {
                    pluginModel.AvailableCustomerRoles.Add(new SelectListItem
                    {
                        Text = role.Name,
                        Value = role.Id.ToString(),
                        Selected = pluginModel.SelectedCustomerRoleIds.Contains(role.Id)
                    });
                }
            }

            //configuration URLs
            if (pluginDescriptor.Installed)
            {
                //display configuration URL only when a plugin is already installed
                var pluginInstance = pluginDescriptor.Instance();
                pluginModel.ConfigurationUrl = pluginInstance.GetConfigurationPageUrl();
                 
                  if (pluginInstance is IExternalAuthenticationMethod)
                {
                    //external auth method
                    pluginModel.CanChangeEnabled = true;
                  
                }
                else if (pluginInstance is IWidgetPlugin)
                {
                    //Misc plugins
                    pluginModel.CanChangeEnabled = true;
                    pluginModel.IsEnabled = ((IWidgetPlugin)pluginInstance).IsWidgetActive(_widgetSettings);
                }

            }
            return pluginModel;
        }

        protected static string GetCategoryBreadCrumbName(OfficialFeedCategory category,
            IList<OfficialFeedCategory> allCategories)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            var breadCrumb = new List<OfficialFeedCategory>();
            while (category != null)
            {
                breadCrumb.Add(category);
                category = allCategories.FirstOrDefault(x => x.Id == category.ParentCategoryId);
            }
            breadCrumb.Reverse();

            var result = "";
            for (int i = 0; i <= breadCrumb.Count - 1; i++)
            {
                result += breadCrumb[i].Name;
                if (i != breadCrumb.Count - 1)
                    result += " >> ";
            }
            return result;
        }
        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new PluginListModel();
            //load modes
            model.AvailableLoadModes = LoadPluginsMode.All.ToSelectList(false).ToList();
            //groups
            model.AvailableGroups.Add(new SelectListItem { Text = ("Admin.Common.All"), Value = "" });
            foreach (var g in _pluginFinder.GetPluginGroups())
                model.AvailableGroups.Add(new SelectListItem { Text = g, Value = g });
            return View(model);
        }

	    [HttpPost]
        public virtual IActionResult ListSelect(DataSourceRequest command, PluginListModel model)
	    {
	        if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
	            return AccessDeniedKendoGridJson();

	        var loadMode = (LoadPluginsMode) model.SearchLoadModeId;
            var pluginDescriptors = _pluginFinder.GetPluginDescriptors(loadMode, group: model.SearchGroup).ToList();
	        var gridModel = new DataSourceResult
            {
                Data = pluginDescriptors.Select(x => PreparePluginModel(x, false, false, false))
                .OrderBy(x => x.Group)
                .ToList(),
                Total = pluginDescriptors.Count()
            };
	        return Json(gridModel);
	    }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "install-plugin-link-")]
        public virtual IActionResult Install(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("install-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue.Substring("install-plugin-link-".Length);

                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is not installed
                if (pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //install plugin
                pluginDescriptor.Instance().Install();

                //activity log
                _customerActivityService.InsertActivity("InstallNewPlugin", ("ActivityLog.InstallNewPlugin"), pluginDescriptor.FriendlyName);

                SuccessNotification(("Admin.Configuration.Plugins.Installed"));

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
             
            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired(FormValueRequirement.StartsWith, "uninstall-plugin-link-")]
        public virtual IActionResult Uninstall(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            try
            {
                //get plugin system name
                string systemName = null;
                foreach (var formValue in form.Keys)
                    if (formValue.StartsWith("uninstall-plugin-link-", StringComparison.InvariantCultureIgnoreCase))
                        systemName = formValue.Substring("uninstall-plugin-link-".Length);

                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
                if (pluginDescriptor == null)
                    //No plugin found with the specified id
                    return RedirectToAction("List");

                //check whether plugin is installed
                if (!pluginDescriptor.Installed)
                    return RedirectToAction("List");

                //uninstall plugin
                pluginDescriptor.Instance().Uninstall();

                //activity log
                _customerActivityService.InsertActivity("UninstallPlugin", ("ActivityLog.UninstallPlugin"), pluginDescriptor.FriendlyName);

                SuccessNotification(("Admin.Configuration.Plugins.Uninstalled"));

                //restart application
                _webHelper.RestartAppDomain();
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("plugin-reload-grid")]
        public virtual IActionResult ReloadList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            //restart application
            _webHelper.RestartAppDomain();
            return RedirectToAction("List");
        }
        
        //edit
        public virtual IActionResult EditPopup(string systemName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(systemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            var model = PreparePluginModel(pluginDescriptor);

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult EditPopup(PluginModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName(model.SystemName, LoadPluginsMode.All);
            if (pluginDescriptor == null)
                //No plugin found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                //we allow editing of 'friendly name', 'display order', store mappings
                pluginDescriptor.FriendlyName = model.FriendlyName;
                pluginDescriptor.DisplayOrder = model.DisplayOrder;
                pluginDescriptor.LimitedToStores.Clear();
                if (model.SelectedStoreIds.Any())
                    pluginDescriptor.LimitedToStores = model.SelectedStoreIds;
                pluginDescriptor.LimitedToCustomerRoles.Clear();
                if (model.SelectedCustomerRoleIds.Any())
                    pluginDescriptor.LimitedToCustomerRoles = model.SelectedCustomerRoleIds;
                PluginFileParser.SavePluginDescriptionFile(pluginDescriptor);
                //reset plugin cache
                _pluginFinder.ReloadPlugins();
               
                //enabled/disabled
                if (pluginDescriptor.Installed)
                {
                    var pluginInstance = pluginDescriptor.Instance();
                   
                      if (pluginInstance is IExternalAuthenticationMethod)
                    {
                        //external auth method
                        var eam = (IExternalAuthenticationMethod)pluginInstance;
                         
                    }
                    else if (pluginInstance is IWidgetPlugin)
                    {
                        //Misc plugins
                        var widget = (IWidgetPlugin)pluginInstance;
                        if (widget.IsWidgetActive(_widgetSettings))
                        {
                            if (!model.IsEnabled)
                            {
                                //mark as disabled
                                _widgetSettings.ActiveWidgetSystemNames.Remove(widget.PluginDescriptor.SystemName);
                                _settingService.SaveSetting(_widgetSettings);
                            }
                        }
                        else
                        {
                            if (model.IsEnabled)
                            {
                                //mark as active
                                _widgetSettings.ActiveWidgetSystemNames.Add(widget.PluginDescriptor.SystemName);
                                _settingService.SaveSetting(_widgetSettings);
                            }
                        }
                    }

                    //activity log
                    _customerActivityService.InsertActivity("EditPlugin", ("ActivityLog.EditPlugin"), pluginDescriptor.FriendlyName);
                }

                ViewBag.RefreshPage = true;
                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //official feed
        public virtual IActionResult OfficialFeed()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var model = new OfficialFeedListModel();
            //versions
            model.AvailableVersions.Add(new SelectListItem { Text = ("Admin.Common.All"), Value = "0" });
            foreach (var version in _officialFeedManager.GetVersions())
                model.AvailableVersions.Add(new SelectListItem{ Text = version.Name, Value = version.Id.ToString()});
            //pre-select current version
            //current version name and named on official site do not match. that's why we use "Contains"
            var currentVersionItem = model.AvailableVersions.FirstOrDefault(x => x.Text.Contains(NopVersion.CurrentVersion));
            if (currentVersionItem != null)
            {
                model.SearchVersionId = int.Parse(currentVersionItem.Value);
                currentVersionItem.Selected = true;
            }

            //categories
            var categories = _officialFeedManager.GetCategories();
            model.AvailableCategories.Add(new SelectListItem { Text = ("Admin.Common.All"), Value = "0" });
            foreach (var category in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = GetCategoryBreadCrumbName(category, categories), Value = category.Id.ToString() });
            //prices
            model.AvailablePrices.Add(new SelectListItem { Text = ("Admin.Common.All"), Value = "0" });
            model.AvailablePrices.Add(new SelectListItem { Text = ("Admin.Configuration.Plugins.OfficialFeed.Price.Free"), Value = "10" });
            model.AvailablePrices.Add(new SelectListItem { Text = ("Admin.Configuration.Plugins.OfficialFeed.Price.Commercial"), Value = "20" });
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult OfficialFeedSelect(DataSourceRequest command, OfficialFeedListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedKendoGridJson();

            var plugins = _officialFeedManager.GetAllPlugins(categoryId: model.SearchCategoryId,
                versionId: model.SearchVersionId,
                price : model.SearchPriceId,
                searchTerm: model.SearchName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult();
            gridModel.Data = plugins.Select(x => new OfficialFeedListModel.ItemOverview
            {
                Url = x.Url,
                Name = x.Name,
                CategoryName = x.Category,
                SupportedVersions = x.SupportedVersions,
                PictureUrl = x.PictureUrl,
                Price = x.Price
            });
            gridModel.Total = plugins.TotalCount;

            return Json(gridModel);
        }

        #endregion
    }
}