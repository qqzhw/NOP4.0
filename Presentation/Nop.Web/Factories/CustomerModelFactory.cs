using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers; 
using Nop.Core.Domain.Media; 
using Nop.Core.Domain.Security;

using Nop.Services.Common;
using Nop.Services.Customers; 
using Nop.Services.Helpers;

using Nop.Services.Media; 

using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the customer model factory
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

       
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly DateTimeSettings _dateTimeSettings;
       
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
      
        private readonly IGenericAttributeService _genericAttributeService;
      
        private readonly CustomerSettings _customerSettings;      
        private readonly IPictureService _pictureService;        
        private readonly IDownloadService _downloadService;
        
        private readonly MediaSettings _mediaSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SecuritySettings _securitySettings;
        
        private readonly CatalogSettings _catalogSettings;
         

        #endregion

        #region Ctor

        public CustomerModelFactory(  
            IDateTimeHelper dateTimeHelper,
            DateTimeSettings dateTimeSettings, 
           
            IWorkContext workContext,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IGenericAttributeService genericAttributeService,
            CustomerSettings customerSettings,
            IPictureService pictureService, 
            IDownloadService downloadService,
           
            MediaSettings mediaSettings,
            CaptchaSettings captchaSettings,
            SecuritySettings securitySettings,
          
            CatalogSettings catalogSettings )
        {
       
            this._dateTimeHelper = dateTimeHelper;
            this._dateTimeSettings = dateTimeSettings;
         
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeMappingService = storeMappingService;
         
            this._genericAttributeService = genericAttributeService;
         
            this._customerSettings = customerSettings;
          
            this._pictureService = pictureService;
           
            this._downloadService = downloadService;
            
            this._mediaSettings = mediaSettings;
            this._captchaSettings = captchaSettings;
            this._securitySettings = securitySettings;
           
            this._catalogSettings = catalogSettings;
           
        }

        #endregion

        #region Methods
 
        /// <summary>
        /// Prepare the customer info model
        /// </summary>
        /// <param name="model">Customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>Customer info model</returns>
        public virtual CustomerInfoModel PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer, 
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

         
            if (!excludeProperties)
            {
             
                model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                var dateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
               
                model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
              
                model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);

             
                model.Signature = customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature);

                model.Email = customer.Email;
                model.Username = customer.Username;
            }
            else
            {
                if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = customer.EmailToRevalidate;
            
          

         
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
        
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
         
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
          
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
         
        

            return model;
        }

      
        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        public virtual LoginModel PrepareLoginModel(bool? checkoutAsGuest)
        {
            var model = new LoginModel();
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault();
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            return model;
        }
        

        /// <summary>
        /// Prepare the customer navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>Customer navigation model</returns>
        public virtual CustomerNavigationModel PrepareCustomerNavigationModel(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = "Account.CustomerInfo",
                Tab = CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title =("Account.CustomerAddresses"),
                Tab = CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = "Account.CustomerOrders",
                Tab = CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

        
            if (!_customerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = "Account.DownloadableProducts",
                    Tab = CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_customerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = "Account.BackInStockSubscriptions",
                    Tab = CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

        
            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = ("Account.ChangePassword"),
                Tab = CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = ("Account.Avatar"),
                    Tab = CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }
                   
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = ("Account.CustomerProductReviews"),
                    Tab = CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
          
            model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return model;
        }

     
        /// <summary>
        /// Prepare the customer downloadable products model
        /// </summary>
        /// <returns>Customer downloadable products model</returns>
        public virtual CustomerDownloadableProductsModel PrepareCustomerDownloadableProductsModel()
        {
            var model = new CustomerDownloadableProductsModel();
           
            return model;
        }

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>User agreement model</returns>
        public virtual UserAgreementModel PrepareUserAgreementModel(Product product)
        {            
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new UserAgreementModel();
            model.UserAgreementText = product.UserAgreementText;
            
            return model;
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        public virtual ChangePasswordModel PrepareChangePasswordModel()
        {
            var model = new ChangePasswordModel();
            return model;
        }

        /// <summary>
        /// Prepare the customer avatar model
        /// </summary>
        /// <param name="model">Customer avatar model</param>
        /// <returns>Customer avatar model</returns>
        public virtual CustomerAvatarModel PrepareCustomerAvatarModel(CustomerAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = _pictureService.GetPictureUrl(
                _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                _mediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        #endregion
    }
}
