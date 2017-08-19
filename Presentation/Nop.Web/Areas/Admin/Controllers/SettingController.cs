using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Domain; 
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
 
using Nop.Core.Domain.Media;
 
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
 
using Nop.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
 
using Nop.Services.Helpers;

using Nop.Services.Logging;
using Nop.Services.Media; 
using Nop.Services.Security;
using Nop.Services.Stores; 
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
	{
		#region Fields

        private readonly ISettingService _settingService;
       
        private readonly IPictureService _pictureService;
       
        private readonly IDateTimeHelper _dateTimeHelper;
        
        private readonly IEncryptionService _encryptionService;
        private readonly IThemeProvider _themeProvider;
        private readonly ICustomerService _customerService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
      
        private readonly IMaintenanceService _maintenanceService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IGenericAttributeService _genericAttributeService;
       
        private readonly NopConfig _config;

        #endregion

        #region Ctor

        public SettingController(ISettingService settingService,
           
            IPictureService pictureService, 
         
            IDateTimeHelper dateTimeHelper,
           
            IEncryptionService encryptionService,
            IThemeProvider themeProvider,
            ICustomerService customerService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
           
            IMaintenanceService maintenanceService,
            IStoreService storeService,
            IWorkContext workContext, 
            IGenericAttributeService genericAttributeService,
          
            NopConfig config)
        {
            this._settingService = settingService;
          
            this._pictureService = pictureService;
           
            this._dateTimeHelper = dateTimeHelper;
           
            this._encryptionService = encryptionService;
            this._themeProvider = themeProvider;
            this._customerService = customerService;
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            
            this._maintenanceService = maintenanceService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._genericAttributeService = genericAttributeService;
            
            this._config = config;
        }

        #endregion

       

        #region Methods

        public virtual IActionResult ChangeStoreScopeConfiguration(int storeid, string returnUrl = "")
        {
            var store = _storeService.GetStoreById(storeid);
            if (store != null || storeid == 0)
            {
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration, storeid);
            }

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = "Admin" });
            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            return Redirect(returnUrl);
        }

     
        
     
        public virtual IActionResult Catalog()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);
            var model = catalogSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AllowViewUnpublishedProductPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowViewUnpublishedProductPage, storeScope);
                model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, storeScope);
                model.ShowSkuOnProductDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowSkuOnProductDetailsPage, storeScope);
                model.ShowSkuOnCatalogPages_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowSkuOnCatalogPages, storeScope);
                model.ShowManufacturerPartNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowManufacturerPartNumber, storeScope);
                model.ShowGtin_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowGtin, storeScope);
                model.ShowFreeShippingNotification_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowFreeShippingNotification, storeScope);
                model.AllowProductSorting_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowProductSorting, storeScope);
                model.AllowProductViewModeChanging_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowProductViewModeChanging, storeScope);
                model.ShowProductsFromSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductsFromSubcategories, storeScope);
                model.ShowCategoryProductNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryProductNumber, storeScope);
                model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, storeScope);
                model.CategoryBreadcrumbEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CategoryBreadcrumbEnabled, storeScope);
                model.ShowShareButton_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowShareButton, storeScope);
                model.PageShareCode_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.PageShareCode, storeScope);
                model.ProductReviewsMustBeApproved_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductReviewsMustBeApproved, storeScope);
                model.AllowAnonymousUsersToReviewProduct_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, storeScope);
                model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, storeScope);
                model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, storeScope);
                model.EmailAFriendEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.EmailAFriendEnabled, storeScope);
                model.AllowAnonymousUsersToEmailAFriend_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, storeScope);
                model.RecentlyViewedProductsNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedProductsNumber, storeScope);
                model.RecentlyViewedProductsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.RecentlyViewedProductsEnabled, storeScope);
                model.NewProductsNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewProductsNumber, storeScope);
                model.NewProductsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NewProductsEnabled, storeScope);
                model.CompareProductsEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.CompareProductsEnabled, storeScope);
                model.ShowBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowBestsellersOnHomepage, storeScope);
                model.NumberOfBestsellersOnHomepage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfBestsellersOnHomepage, storeScope);
                model.SearchPageProductsPerPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageProductsPerPage, storeScope);
                model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, storeScope);
                model.SearchPagePageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.SearchPagePageSizeOptions, storeScope);
                model.ProductSearchAutoCompleteEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, storeScope);
                model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, storeScope);
                model.ShowProductImagesInSearchAutoComplete_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, storeScope);
                model.ProductSearchTermMinimumLength_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductSearchTermMinimumLength, storeScope);
                model.ProductsAlsoPurchasedEnabled_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, storeScope);
                model.ProductsAlsoPurchasedNumber_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsAlsoPurchasedNumber, storeScope);
                model.NumberOfProductTags_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.NumberOfProductTags, storeScope);
                model.ProductsByTagPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsByTagPageSize, storeScope);
                model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, storeScope);
                model.ProductsByTagPageSizeOptions_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ProductsByTagPageSizeOptions, storeScope);
                model.IncludeShortDescriptionInCompareProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, storeScope);
                model.IncludeFullDescriptionInCompareProducts_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, storeScope);
                model.ManufacturersBlockItemsToDisplay_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, storeScope);
                model.DisplayTaxShippingInfoFooter_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoFooter, storeScope);
                model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, storeScope);
                model.DisplayTaxShippingInfoProductBoxes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, storeScope);
                model.DisplayTaxShippingInfoShoppingCart_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, storeScope);
                model.DisplayTaxShippingInfoWishlist_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, storeScope);
                model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, storeScope);
                model.ShowProductReviewsPerStore_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductReviewsPerStore, storeScope);
                model.ShowProductReviewsOnAccountPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, storeScope);
                model.ProductReviewsPageSizeOnAccountPage_OverrideForStore = _settingService.SettingExists(catalogSettings, x=> x.ProductReviewsPageSizeOnAccountPage, storeScope);
                model.ExportImportProductAttributes_OverrideForStore = _settingService.SettingExists(catalogSettings, x => x.ExportImportProductAttributes, storeScope);
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult Catalog(CatalogSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);
            catalogSettings = model.ToEntity(catalogSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowViewUnpublishedProductPage, model.AllowViewUnpublishedProductPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedProducts, model.DisplayDiscontinuedMessageForUnpublishedProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowSkuOnProductDetailsPage, model.ShowSkuOnProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowSkuOnCatalogPages, model.ShowSkuOnCatalogPages_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowManufacturerPartNumber, model.ShowManufacturerPartNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowGtin, model.ShowGtin_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowFreeShippingNotification, model.ShowFreeShippingNotification_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowProductSorting, model.AllowProductSorting_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowProductViewModeChanging, model.AllowProductViewModeChanging_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductsFromSubcategories, model.ShowProductsFromSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowCategoryProductNumber, model.ShowCategoryProductNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowCategoryProductNumberIncludingSubcategories, model.ShowCategoryProductNumberIncludingSubcategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.CategoryBreadcrumbEnabled, model.CategoryBreadcrumbEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowShareButton, model.ShowShareButton_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.PageShareCode, model.PageShareCode_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductReviewsMustBeApproved, model.ProductReviewsMustBeApproved_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowAnonymousUsersToReviewProduct, model.AllowAnonymousUsersToReviewProduct_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductReviewPossibleOnlyAfterPurchasing, model.ProductReviewPossibleOnlyAfterPurchasing_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NotifyStoreOwnerAboutNewProductReviews, model.NotifyStoreOwnerAboutNewProductReviews_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.EmailAFriendEnabled, model.EmailAFriendEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.RecentlyViewedProductsNumber, model.RecentlyViewedProductsNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.RecentlyViewedProductsEnabled, model.RecentlyViewedProductsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NewProductsNumber, model.NewProductsNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NewProductsEnabled, model.NewProductsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.CompareProductsEnabled, model.CompareProductsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowBestsellersOnHomepage, model.ShowBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NumberOfBestsellersOnHomepage, model.NumberOfBestsellersOnHomepage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPageProductsPerPage, model.SearchPageProductsPerPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPageAllowCustomersToSelectPageSize, model.SearchPageAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.SearchPagePageSizeOptions, model.SearchPagePageSizeOptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductSearchAutoCompleteEnabled, model.ProductSearchAutoCompleteEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductSearchAutoCompleteNumberOfProducts, model.ProductSearchAutoCompleteNumberOfProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductImagesInSearchAutoComplete, model.ShowProductImagesInSearchAutoComplete_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductSearchTermMinimumLength, model.ProductSearchTermMinimumLength_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsAlsoPurchasedEnabled, model.ProductsAlsoPurchasedEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsAlsoPurchasedNumber, model.ProductsAlsoPurchasedNumber_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.NumberOfProductTags, model.NumberOfProductTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsByTagPageSize, model.ProductsByTagPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsByTagAllowCustomersToSelectPageSize, model.ProductsByTagAllowCustomersToSelectPageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductsByTagPageSizeOptions, model.ProductsByTagPageSizeOptions_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.IncludeShortDescriptionInCompareProducts, model.IncludeShortDescriptionInCompareProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.IncludeFullDescriptionInCompareProducts, model.IncludeFullDescriptionInCompareProducts_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, model.ManufacturersBlockItemsToDisplay_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoFooter, model.DisplayTaxShippingInfoFooter_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoProductDetailsPage, model.DisplayTaxShippingInfoProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoProductBoxes, model.DisplayTaxShippingInfoProductBoxes_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, model.DisplayTaxShippingInfoWishlist_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductReviewsPerStore, model.ShowProductReviewsPerStore_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ShowProductReviewsTabOnAccountPage, model.ShowProductReviewsOnAccountPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ProductReviewsPageSizeOnAccountPage, model.ProductReviewsPageSizeOnAccountPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(catalogSettings, x => x.ExportImportProductAttributes, model.ExportImportProductAttributes_OverrideForStore, storeScope, false);
            
            //now settings not overridable per store
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreDiscounts, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreFeaturedProducts, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreAcl, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.IgnoreStoreLimitations, 0, false);
            _settingService.SaveSetting(catalogSettings, x => x.CacheProductPrices, 0, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", ("ActivityLog.EditSettings"));

            SuccessNotification(("Admin.Configuration.Updated"));

            return RedirectToAction("Catalog");
        }

        #region Sort options

        [HttpPost]
        public virtual IActionResult SortOptionsList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var catalogSettings = _settingService.LoadSetting<CatalogSettings>();
            var model = new List<SortOptionModel>();
            foreach (int option in Enum.GetValues(typeof(ProductSortingEnum)))
            {
                int value;
                model.Add(new SortOptionModel()
                {
                    Id = option,
                    Name = ((ProductSortingEnum)option).ToString(),
                    IsActive = !catalogSettings.ProductSortingEnumDisabled.Contains(option),
                    DisplayOrder = catalogSettings.ProductSortingEnumDisplayOrder.TryGetValue(option, out value) ? value : option
                });
            }
            var gridModel = new DataSourceResult
            {
                Data = model.OrderBy(option => option.DisplayOrder),
                Total = model.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult SortOptionUpdate(SortOptionModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var catalogSettings = _settingService.LoadSetting<CatalogSettings>(storeScope);

            catalogSettings.ProductSortingEnumDisplayOrder[model.Id] = model.DisplayOrder;
            if (model.IsActive && catalogSettings.ProductSortingEnumDisabled.Contains(model.Id))
                catalogSettings.ProductSortingEnumDisabled.Remove(model.Id);
            if (!model.IsActive && !catalogSettings.ProductSortingEnumDisabled.Contains(model.Id))
                catalogSettings.ProductSortingEnumDisabled.Add(model.Id);


            _settingService.SaveSetting(catalogSettings);

            return new NullJsonResult();
        }

        #endregion
  
         
        public virtual IActionResult Media()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            var model = mediaSettings.ToModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AvatarPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AvatarPictureSize, storeScope);
                model.ProductThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSize, storeScope);
                model.ProductDetailsPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductDetailsPictureSize, storeScope);
                model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, storeScope);
                model.AssociatedProductPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.AssociatedProductPictureSize, storeScope);
                model.CategoryThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CategoryThumbPictureSize, storeScope);
                model.ManufacturerThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ManufacturerThumbPictureSize, storeScope);
                model.VendorThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.VendorThumbPictureSize, storeScope);
                model.CartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.CartThumbPictureSize, storeScope);
                model.MiniCartThumbPictureSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MiniCartThumbPictureSize, storeScope);
                model.MaximumImageSize_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MaximumImageSize, storeScope);
                model.MultipleThumbDirectories_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.MultipleThumbDirectories, storeScope);
                model.DefaultImageQuality_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultImageQuality, storeScope);
                model.ImportProductImagesUsingHash_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.ImportProductImagesUsingHash, storeScope);
                model.DefaultPictureZoomEnabled_OverrideForStore = _settingService.SettingExists(mediaSettings, x => x.DefaultPictureZoomEnabled, storeScope);
            }
            model.PicturesStoredIntoDatabase = _pictureService.StoreInDb;

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult Media(MediaSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var mediaSettings = _settingService.LoadSetting<MediaSettings>(storeScope);
            mediaSettings = model.ToEntity(mediaSettings);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductThumbPictureSize, model.ProductThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductDetailsPictureSize, model.ProductDetailsPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ProductThumbPictureSizeOnProductDetailsPage, model.ProductThumbPictureSizeOnProductDetailsPage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.AssociatedProductPictureSize, model.AssociatedProductPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CategoryThumbPictureSize, model.CategoryThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ManufacturerThumbPictureSize, model.ManufacturerThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.VendorThumbPictureSize, model.VendorThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.CartThumbPictureSize, model.CartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MiniCartThumbPictureSize, model.MiniCartThumbPictureSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.MultipleThumbDirectories, model.MultipleThumbDirectories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.ImportProductImagesUsingHash, model.ImportProductImagesUsingHash_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(mediaSettings, x => x.DefaultPictureZoomEnabled, model.DefaultPictureZoomEnabled_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();
            
            //activity log
            _customerActivityService.InsertActivity("EditSettings", ("ActivityLog.EditSettings"));

            SuccessNotification(("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }

        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public virtual IActionResult ChangePictureStorage()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            _pictureService.StoreInDb = !_pictureService.StoreInDb;

            //activity log
            _customerActivityService.InsertActivity("EditSettings", ("ActivityLog.EditSettings"));

            SuccessNotification(("Admin.Configuration.Updated"));
            return RedirectToAction("Media");
        }
        
        public virtual IActionResult CustomerUser()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
           
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);

            //merge settings
            var model = new CustomerUserSettingsModel();
            model.CustomerSettings = customerSettings.ToModel();
         
            model.DateTimeSettings.AllowCustomersToSetTimeZone = dateTimeSettings.AllowCustomersToSetTimeZone;
            model.DateTimeSettings.DefaultStoreTimeZoneId = _dateTimeHelper.DefaultStoreTimeZone.Id;
            foreach (TimeZoneInfo timeZone in _dateTimeHelper.GetSystemTimeZones())
            {
                model.DateTimeSettings.AvailableTimeZones.Add(new SelectListItem
                    {
                        Text = timeZone.DisplayName,
                        Value = timeZone.Id,
                        Selected = timeZone.Id.Equals(_dateTimeHelper.DefaultStoreTimeZone.Id, StringComparison.InvariantCultureIgnoreCase)
                    });
            }

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult CustomerUser(CustomerUserSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();


            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var customerSettings = _settingService.LoadSetting<CustomerSettings>(storeScope);
         
            var dateTimeSettings = _settingService.LoadSetting<DateTimeSettings>(storeScope);

            customerSettings = model.CustomerSettings.ToEntity(customerSettings);
            _settingService.SaveSetting(customerSettings);
            
            dateTimeSettings.DefaultStoreTimeZoneId = model.DateTimeSettings.DefaultStoreTimeZoneId;
            dateTimeSettings.AllowCustomersToSetTimeZone = model.DateTimeSettings.AllowCustomersToSetTimeZone;
            _settingService.SaveSetting(dateTimeSettings);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", ("ActivityLog.EditSettings"));

            SuccessNotification(("Admin.Configuration.Updated"));

            //selected tab
            SaveSelectedTabName();

            return RedirectToAction("CustomerUser");
        }
        
        public virtual IActionResult GeneralCommon()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var model = new GeneralCommonSettingsModel();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            model.ActiveStoreScopeConfiguration = storeScope;
            //store information
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            model.StoreInformationSettings.StoreClosed = storeInformationSettings.StoreClosed;
            //themes
            model.StoreInformationSettings.DefaultStoreTheme = storeInformationSettings.DefaultStoreTheme;
            model.StoreInformationSettings.AvailableStoreThemes = _themeProvider
                .GetThemeConfigurations()
                .Select(x => new GeneralCommonSettingsModel.StoreInformationSettingsModel.ThemeConfigurationModel
                {
                    ThemeTitle = x.ThemeTitle,
                    ThemeName = x.ThemeName,
                    PreviewImageUrl = x.PreviewImageUrl,
                    PreviewText = x.PreviewText,
                    SupportRtl = x.SupportRtl,
                    Selected = x.ThemeName.Equals(storeInformationSettings.DefaultStoreTheme, StringComparison.InvariantCultureIgnoreCase)
                })
                .ToList();

            model.StoreInformationSettings.AllowCustomerToSelectTheme = storeInformationSettings.AllowCustomerToSelectTheme;
            model.StoreInformationSettings.LogoPictureId = storeInformationSettings.LogoPictureId;
            //EU Cookie law
            model.StoreInformationSettings.DisplayEuCookieLawWarning = storeInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            model.StoreInformationSettings.FacebookLink = storeInformationSettings.FacebookLink;
            model.StoreInformationSettings.TwitterLink = storeInformationSettings.TwitterLink;
            model.StoreInformationSettings.YoutubeLink = storeInformationSettings.YoutubeLink;
            model.StoreInformationSettings.GooglePlusLink = storeInformationSettings.GooglePlusLink;
            //contact us
            model.StoreInformationSettings.SubjectFieldOnContactUsForm = commonSettings.SubjectFieldOnContactUsForm;
            model.StoreInformationSettings.UseSystemEmailForContactUsForm = commonSettings.UseSystemEmailForContactUsForm;
            //sitemap
            model.StoreInformationSettings.SitemapEnabled = commonSettings.SitemapEnabled;
            model.StoreInformationSettings.SitemapIncludeCategories = commonSettings.SitemapIncludeCategories;
            model.StoreInformationSettings.SitemapIncludeManufacturers = commonSettings.SitemapIncludeManufacturers;
            model.StoreInformationSettings.SitemapIncludeProducts = commonSettings.SitemapIncludeProducts;

            //override settings
            if (storeScope > 0)
            {
                model.StoreInformationSettings.StoreClosed_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.StoreClosed, storeScope);
                model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DefaultStoreTheme, storeScope);
                model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.AllowCustomerToSelectTheme, storeScope);
                model.StoreInformationSettings.LogoPictureId_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.LogoPictureId, storeScope);
                model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.DisplayEuCookieLawWarning, storeScope);
                model.StoreInformationSettings.FacebookLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.FacebookLink, storeScope);
                model.StoreInformationSettings.TwitterLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.TwitterLink, storeScope);
                model.StoreInformationSettings.YoutubeLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.YoutubeLink, storeScope);
                model.StoreInformationSettings.GooglePlusLink_OverrideForStore = _settingService.SettingExists(storeInformationSettings, x => x.GooglePlusLink, storeScope);
                model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SubjectFieldOnContactUsForm, storeScope);
                model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.UseSystemEmailForContactUsForm, storeScope);
                model.StoreInformationSettings.SitemapEnabled_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapEnabled, storeScope);
                model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeCategories, storeScope);
                model.StoreInformationSettings.SitemapIncludeManufacturers_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeManufacturers, storeScope);
                model.StoreInformationSettings.SitemapIncludeProducts_OverrideForStore = _settingService.SettingExists(commonSettings, x => x.SitemapIncludeProducts, storeScope);
            }

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            model.SeoSettings.PageTitleSeparator = seoSettings.PageTitleSeparator;
            model.SeoSettings.PageTitleSeoAdjustment = (int)seoSettings.PageTitleSeoAdjustment;
            model.SeoSettings.PageTitleSeoAdjustmentValues = seoSettings.PageTitleSeoAdjustment.ToSelectList();
            model.SeoSettings.DefaultTitle = seoSettings.DefaultTitle;
            model.SeoSettings.DefaultMetaKeywords = seoSettings.DefaultMetaKeywords;
            model.SeoSettings.DefaultMetaDescription = seoSettings.DefaultMetaDescription;
            model.SeoSettings.GenerateProductMetaDescription = seoSettings.GenerateProductMetaDescription;
            model.SeoSettings.ConvertNonWesternChars = seoSettings.ConvertNonWesternChars;
            model.SeoSettings.CanonicalUrlsEnabled = seoSettings.CanonicalUrlsEnabled;
            model.SeoSettings.WwwRequirement = (int)seoSettings.WwwRequirement;
            model.SeoSettings.WwwRequirementValues = seoSettings.WwwRequirement.ToSelectList();
            model.SeoSettings.EnableJsBundling = seoSettings.EnableJsBundling;
            model.SeoSettings.EnableCssBundling = seoSettings.EnableCssBundling;
            model.SeoSettings.TwitterMetaTags = seoSettings.TwitterMetaTags;
            model.SeoSettings.OpenGraphMetaTags = seoSettings.OpenGraphMetaTags;
            model.SeoSettings.CustomHeadTags = seoSettings.CustomHeadTags;
            //override settings
            if (storeScope > 0)
            {
                model.SeoSettings.PageTitleSeparator_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeparator, storeScope);
                model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.PageTitleSeoAdjustment, storeScope);
                model.SeoSettings.DefaultTitle_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultTitle, storeScope);
                model.SeoSettings.DefaultMetaKeywords_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaKeywords, storeScope);
                model.SeoSettings.DefaultMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.DefaultMetaDescription, storeScope);
                model.SeoSettings.GenerateProductMetaDescription_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.GenerateProductMetaDescription, storeScope);
                model.SeoSettings.ConvertNonWesternChars_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.ConvertNonWesternChars, storeScope);
                model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CanonicalUrlsEnabled, storeScope);
                model.SeoSettings.WwwRequirement_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.WwwRequirement, storeScope);
                model.SeoSettings.EnableJsBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableJsBundling, storeScope);
                model.SeoSettings.EnableCssBundling_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.EnableCssBundling, storeScope);
                model.SeoSettings.TwitterMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.TwitterMetaTags, storeScope);
                model.SeoSettings.OpenGraphMetaTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.OpenGraphMetaTags, storeScope);
                model.SeoSettings.CustomHeadTags_OverrideForStore = _settingService.SettingExists(seoSettings, x => x.CustomHeadTags, storeScope);
            }
            
            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            model.SecuritySettings.EncryptionKey = securitySettings.EncryptionKey;
            if (securitySettings.AdminAreaAllowedIpAddresses != null)
                for (int i = 0; i < securitySettings.AdminAreaAllowedIpAddresses.Count; i++)
                {
                    model.SecuritySettings.AdminAreaAllowedIpAddresses += securitySettings.AdminAreaAllowedIpAddresses[i];
                    if (i != securitySettings.AdminAreaAllowedIpAddresses.Count - 1)
                        model.SecuritySettings.AdminAreaAllowedIpAddresses += ",";
                }
            model.SecuritySettings.ForceSslForAllPages = securitySettings.ForceSslForAllPages;
            model.SecuritySettings.EnableXsrfProtectionForAdminArea = securitySettings.EnableXsrfProtectionForAdminArea;
            model.SecuritySettings.EnableXsrfProtectionForPublicStore = securitySettings.EnableXsrfProtectionForPublicStore;
            model.SecuritySettings.HoneypotEnabled = securitySettings.HoneypotEnabled;

            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            model.CaptchaSettings = captchaSettings.ToModel();
            model.CaptchaSettings.AvailableReCaptchaVersions = ReCaptchaVersion.Version1.ToSelectList(false).ToList();
                      
            //override settings
            if (storeScope > 0)
            {
             }

           
            //full-text support
          
            model.FullTextSettings.SearchMode = (int)commonSettings.FullTextMode;
            model.FullTextSettings.SearchModeValues = commonSettings.FullTextMode.ToSelectList();
            
            //display default menu item
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem = displayDefaultMenuItemSettings.DisplayHomePageMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem = displayDefaultMenuItemSettings.DisplayNewProductsMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem = displayDefaultMenuItemSettings.DisplayProductSearchMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem = displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem = displayDefaultMenuItemSettings.DisplayBlogMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem = displayDefaultMenuItemSettings.DisplayForumsMenuItem;
            model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem = displayDefaultMenuItemSettings.DisplayContactUsMenuItem;

            model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayHomePageMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayCustomerInfoMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, storeScope);
            model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore = _settingService.SettingExists(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, storeScope);

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual IActionResult GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);

            //store information settings
            var storeInformationSettings = _settingService.LoadSetting<StoreInformationSettings>(storeScope);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
            storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
            storeInformationSettings.AllowCustomerToSelectTheme = model.StoreInformationSettings.AllowCustomerToSelectTheme;
            storeInformationSettings.LogoPictureId = model.StoreInformationSettings.LogoPictureId;
            //EU Cookie law
            storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
            //social pages
            storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
            storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
            storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
            storeInformationSettings.GooglePlusLink = model.StoreInformationSettings.GooglePlusLink;
            //contact us
            commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
            commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;
            //sitemap
            commonSettings.SitemapEnabled = model.StoreInformationSettings.SitemapEnabled;
            commonSettings.SitemapIncludeCategories = model.StoreInformationSettings.SitemapIncludeCategories;
            commonSettings.SitemapIncludeManufacturers = model.StoreInformationSettings.SitemapIncludeManufacturers;
            commonSettings.SitemapIncludeProducts = model.StoreInformationSettings.SitemapIncludeProducts;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.StoreClosed, model.StoreInformationSettings.StoreClosed_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DefaultStoreTheme, model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.AllowCustomerToSelectTheme, model.StoreInformationSettings.AllowCustomerToSelectTheme_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.LogoPictureId, model.StoreInformationSettings.LogoPictureId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.DisplayEuCookieLawWarning, model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.FacebookLink, model.StoreInformationSettings.FacebookLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.TwitterLink, model.StoreInformationSettings.TwitterLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.YoutubeLink, model.StoreInformationSettings.YoutubeLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(storeInformationSettings, x => x.GooglePlusLink, model.StoreInformationSettings.GooglePlusLink_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SubjectFieldOnContactUsForm, model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.UseSystemEmailForContactUsForm, model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapEnabled, model.StoreInformationSettings.SitemapEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeCategories, model.StoreInformationSettings.SitemapIncludeCategories_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeManufacturers, model.StoreInformationSettings.SitemapIncludeManufacturers_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(commonSettings, x => x.SitemapIncludeProducts, model.StoreInformationSettings.SitemapIncludeProducts_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //seo settings
            var seoSettings = _settingService.LoadSetting<SeoSettings>(storeScope);
            seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
            seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
            seoSettings.DefaultTitle = model.SeoSettings.DefaultTitle;
            seoSettings.DefaultMetaKeywords = model.SeoSettings.DefaultMetaKeywords;
            seoSettings.DefaultMetaDescription = model.SeoSettings.DefaultMetaDescription;
            seoSettings.GenerateProductMetaDescription = model.SeoSettings.GenerateProductMetaDescription;
            seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
            seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
            seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
            seoSettings.EnableJsBundling = model.SeoSettings.EnableJsBundling;
            seoSettings.EnableCssBundling = model.SeoSettings.EnableCssBundling;
            seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
            seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
            seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeparator, model.SeoSettings.PageTitleSeparator_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.PageTitleSeoAdjustment, model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultTitle, model.SeoSettings.DefaultTitle_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaKeywords, model.SeoSettings.DefaultMetaKeywords_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.DefaultMetaDescription, model.SeoSettings.DefaultMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.GenerateProductMetaDescription, model.SeoSettings.GenerateProductMetaDescription_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.ConvertNonWesternChars, model.SeoSettings.ConvertNonWesternChars_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CanonicalUrlsEnabled, model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.WwwRequirement, model.SeoSettings.WwwRequirement_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableJsBundling, model.SeoSettings.EnableJsBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.EnableCssBundling, model.SeoSettings.EnableCssBundling_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.TwitterMetaTags, model.SeoSettings.TwitterMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.OpenGraphMetaTags, model.SeoSettings.OpenGraphMetaTags_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(seoSettings, x => x.CustomHeadTags, model.SeoSettings.CustomHeadTags_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            _settingService.ClearCache();

            //security settings
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);
            if (securitySettings.AdminAreaAllowedIpAddresses == null)
                securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
            securitySettings.AdminAreaAllowedIpAddresses.Clear();
            if (!String.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                foreach (string s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    if (!String.IsNullOrWhiteSpace(s))
                        securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
            securitySettings.ForceSslForAllPages = model.SecuritySettings.ForceSslForAllPages;
            securitySettings.EnableXsrfProtectionForAdminArea = model.SecuritySettings.EnableXsrfProtectionForAdminArea;
            securitySettings.EnableXsrfProtectionForPublicStore = model.SecuritySettings.EnableXsrfProtectionForPublicStore;
            securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
            _settingService.SaveSetting(securitySettings);

            //captcha settings
            var captchaSettings = _settingService.LoadSetting<CaptchaSettings>(storeScope);
            captchaSettings = model.CaptchaSettings.ToEntity(captchaSettings);
            _settingService.SaveSetting(captchaSettings);
            if (captchaSettings.Enabled &&
                (String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || String.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
            {
                //captcha is enabled but the keys are not entered
                ErrorNotification(("Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError"));
            }
                      
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            
           
            //now clear settings cache
            _settingService.ClearCache();
            

            //full-text
            commonSettings.FullTextMode = (FulltextSearchMode)model.FullTextSettings.SearchMode;
            _settingService.SaveSetting(commonSettings);

            //display default menu item
            var displayDefaultMenuItemSettings = _settingService.LoadSetting<DisplayDefaultMenuItemSettings>(storeScope);

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            displayDefaultMenuItemSettings.DisplayHomePageMenuItem = model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem;
            displayDefaultMenuItemSettings.DisplayNewProductsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem;
            displayDefaultMenuItemSettings.DisplayProductSearchMenuItem = model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem;
            displayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem = model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem;
            displayDefaultMenuItemSettings.DisplayBlogMenuItem = model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem;
            displayDefaultMenuItemSettings.DisplayForumsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem;
            displayDefaultMenuItemSettings.DisplayContactUsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem;

            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayHomePageMenuItem, model.DisplayDefaultMenuItemSettings.DisplayHomePageMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayNewProductsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayNewProductsMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayProductSearchMenuItem, model.DisplayDefaultMenuItemSettings.DisplayProductSearchMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayCustomerInfoMenuItem, model.DisplayDefaultMenuItemSettings.DisplayCustomerInfoMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //activity log
            _customerActivityService.InsertActivity("EditSettings", ("ActivityLog.EditSettings"));

            SuccessNotification(("Admin.Configuration.Updated"));
            
            return RedirectToAction("GeneralCommon");
        }

        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public virtual IActionResult ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var securitySettings = _settingService.LoadSetting<SecuritySettings>(storeScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = "";

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (String.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new NopException(("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                string oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new NopException(("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));
                 
                //update password information
                //optimization - load only passwords with PasswordFormat.Encrypted
                var customerPasswords = _customerService.GetCustomerPasswords(passwordFormat: PasswordFormat.Encrypted);
                foreach (var customerPassword in customerPasswords)
                {
                    var decryptedPassword = _encryptionService.DecryptText(customerPassword.Password, oldEncryptionPrivateKey);
                    var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    customerPassword.Password = encryptedPassword;
                    _customerService.UpdateCustomerPassword(customerPassword);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                _settingService.SaveSetting(securitySettings);
                SuccessNotification(("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("GeneralCommon");
        }

        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("togglefulltext")]
        public virtual IActionResult ToggleFullText(GeneralCommonSettingsModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var commonSettings = _settingService.LoadSetting<CommonSettings>(storeScope);
            try
            { 
                if (commonSettings.UseFullTextSearch)
                { 
                    commonSettings.UseFullTextSearch = false;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Disabled"));
                }
                else
                { 
                    commonSettings.UseFullTextSearch = true;
                    _settingService.SaveSetting(commonSettings);

                    SuccessNotification(("Admin.Configuration.Settings.GeneralCommon.FullTextSettings.Enabled"));
                }
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }
            
            return RedirectToAction("GeneralCommon");
        }
        
        //all settings
        public virtual IActionResult AllSettings()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();
            
            return View();
        }

        [HttpPost]
        public virtual IActionResult AllSettings(DataSourceRequest command, AllSettingsListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedKendoGridJson();

            var query = _settingService.GetAllSettings().AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchSettingName))
                query = query.Where(s => s.Name.ToLowerInvariant().Contains(model.SearchSettingName.ToLowerInvariant()));
            if (!string.IsNullOrEmpty(model.SearchSettingValue))
                query = query.Where(s => s.Value.ToLowerInvariant().Contains(model.SearchSettingValue.ToLowerInvariant()));

            var settings = query.ToList()
                .Select(x =>
                            {
                                string storeName;
                                if (x.StoreId == 0)
                                {
                                    storeName = ("Admin.Configuration.Settings.AllSettings.Fields.StoreName.AllStores");
                                }
                                else
                                {
                                    var store = _storeService.GetStoreById(x.StoreId);
                                    storeName = store != null ? store.Name : "Unknown";
                                }
                                var settingModel = new SettingModel
                                {
                                    Id = x.Id,
                                    Name = x.Name,
                                    Value = x.Value,
                                    Store = storeName,
                                    StoreId = x.StoreId
                                };
                                return settingModel;
                            })
                .AsQueryable();

            var gridModel = new DataSourceResult
            {
                Data = settings.PagedForCommand(command).ToList(),
                Total = settings.Count()
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult SettingUpdate(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var setting = _settingService.GetSettingById(model.Id);
            if (setting == null)
                return Content("No setting could be loaded with the specified ID");

            var storeId = model.StoreId;

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase) ||
                setting.StoreId != storeId)
            {
                //setting name or store has been changed
                _settingService.DeleteSetting(setting);
            }

            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("EditSettings", ("ActivityLog.EditSettings"));

            return new NullJsonResult();
        }
        [HttpPost]
        public virtual IActionResult SettingAdd(SettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();
            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }
            var storeId = model.StoreId;
            _settingService.SetSetting(model.Name, model.Value, storeId);

            //activity log
            _customerActivityService.InsertActivity("AddNewSetting", ("ActivityLog.AddNewSetting"), model.Name);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult SettingDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var setting = _settingService.GetSettingById(id);
            if (setting == null)
                throw new ArgumentException("No setting found with the specified id");
            _settingService.DeleteSetting(setting);

            //activity log
            _customerActivityService.InsertActivity("DeleteSetting", ("ActivityLog.DeleteSetting"), setting.Name);

            return new NullJsonResult();
        }

        //action displaying notification (warning) to a store owner about a lot of traffic 
        //between the Redis server and the application when LoadAllLocaleRecordsOnStartup seetting is set
        public IActionResult RedisCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
        {
            //LoadAllLocaleRecordsOnStartup is set and Redis cache is used, so display warning
            if (_config.RedisCachingEnabled && loadAllLocaleRecordsOnStartup)
                return Json(new
                {
                    Result = (
                        "Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning")
                });

            return Json(new { Result = string.Empty });
        }

        #endregion
    }
}