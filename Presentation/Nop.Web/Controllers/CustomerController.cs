using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Services.Authentication;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Helpers;

using Nop.Services.Logging;
using Nop.Services.Media;

using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Models.Customer;

namespace Nop.Web.Controllers
{
    public partial class CustomerController : BasePublicController
    {
        #region Fields

      
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IAuthenticationService _authenticationService;
        private readonly DateTimeSettings _dateTimeSettings; 
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ICustomerService _customerService; 
        private readonly IGenericAttributeService _genericAttributeService; 
        private readonly CustomerSettings _customerSettings; 
        private readonly IPictureService _pictureService;      
        private readonly IWebHelper _webHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IEventPublisher _eventPublisher;

        private readonly MediaSettings _mediaSettings;
        
        private readonly CaptchaSettings _captchaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;

        #endregion

        #region Ctor

        public CustomerController(
            ICustomerRegistrationService  customerRegistrationService,
        ICustomerModelFactory customerModelFactory,
            IAuthenticationService authenticationService,
            DateTimeSettings dateTimeSettings,          
            IWorkContext workContext,
            IStoreContext storeContext,
            ICustomerService customerService,           
            IGenericAttributeService genericAttributeService,       
            CustomerSettings customerSettings,           
            IPictureService pictureService,         
      
            IWebHelper webHelper,
            ICustomerActivityService customerActivityService,           
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,         
            CaptchaSettings captchaSettings,
            StoreInformationSettings storeInformationSettings)
        {
            _customerRegistrationService = customerRegistrationService;
            this._customerModelFactory = customerModelFactory;
            this._authenticationService = authenticationService;
            this._dateTimeSettings = dateTimeSettings;           
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._customerService = customerService;          
            this._genericAttributeService = genericAttributeService;         
            this._customerSettings = customerSettings;        
            this._pictureService = pictureService;         
         
            this._webHelper = webHelper;
            this._customerActivityService = customerActivityService;        
            this._eventPublisher = eventPublisher;
            this._mediaSettings = mediaSettings;        
            this._captchaSettings = captchaSettings;
            this._storeInformationSettings = storeInformationSettings;
        }

        #endregion

       

        #region Methods

        #region Login / logout

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage());
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username)
                                : _customerService.GetCustomerByEmail(model.Email);

                           
                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login", ("ActivityLog.PublicStore.Login"));

                            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("HomePage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", "Account.Login.WrongCredentials.CustomerNotExist");
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", ("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", ("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", ("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", ("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", ("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Logout()
        {
            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated,
                    "Impersonation.Finished",
                    ("ActivityLog.Impersonation.Finished.StoreOwner"),
                    _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id);
                _customerActivityService.InsertActivity("Impersonation.Finished",
                 "ActivityLog.Impersonation.Finished.Customer",
                    _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id);

                //logout impersonated customer
                _genericAttributeService.SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated,
                    SystemCustomerAttributeNames.ImpersonatedCustomerId, null);

                //redirect back to customer details page (admin area)
                return this.RedirectToAction("Edit", "Customer",
                    new { id = _workContext.CurrentCustomer.Id, area = "Admin" });

            }

            //activity log
            _customerActivityService.InsertActivity("PublicStore.Logout", ("ActivityLog.PublicStore.Logout"));

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));
			 
            return RedirectToRoute("HomePage");
        }

        #endregion
          

        #region Register
 

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [HttpPost]
        public virtual IActionResult RegisterResult(string returnUrl)
        {
            if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("HomePage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        [PublicAntiForgery]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = ("Account.CheckUsernameAvailability.NotAvailable");

            if (_customerSettings.UsernamesEnabled && !String.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentCustomer != null &&
                    _workContext.CurrentCustomer.Username != null &&
                    _workContext.CurrentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = ("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var customer = _customerService.GetCustomerByUsername(username);
                    if (customer == null)
                    {
                        statusText = ("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        [HttpsRequirement(SslRequirement.Yes)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult AccountActivation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cToken = customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken);
            if (string.IsNullOrEmpty(cToken))
                return
                    View(new AccountActivationModel
                    {
                        Result = ("Account.AccountActivation.AlreadyActivated")
                    });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, "");
            
            var model = new AccountActivationModel();
            model.Result = ("Account.AccountActivation.Activated");
            return View(model);
        }
        #endregion

        #region My account / Info

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            var model = new CustomerInfoModel();
            model = _customerModelFactory.PrepareCustomerInfoModel(model, _workContext.CurrentCustomer, false);

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult Info(CustomerInfoModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

          
            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_customerSettings.UsernamesEnabled && this._customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (
                            !customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                           
                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalCustomerIfImpersonated == null)
                                _authenticationService.SignIn(customer, true);
                        }
                    }
                    //email
                    if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                      
                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_customerSettings.UsernamesEnabled)
                                _authenticationService.SignIn(customer, true);
                        }
                    }

                   
                   

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Gender, model.Gender);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        DateTime? dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.DateOfBirth, dateOfBirth);
                    }
                 
              
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.City, model.City);
                   
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StateProvinceId, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Phone, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Fax, model.Fax);

                 
 
                    
                    return RedirectToRoute("CustomerInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }


            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerInfoModel(model, customer, true);
            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult RemoveExternalAssociation(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();
                    
      
            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

      
        #endregion

       
        #region My account / Downloadable products

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult DownloadableProducts()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            if (_customerSettings.HideDownloadableProductsTab)
                return RedirectToRoute("CustomerInfo");

            var model = _customerModelFactory.PrepareCustomerDownloadableProductsModel();
            return View(model);
        }

      
        #endregion

        #region My account / Change password

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult ChangePassword()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            var model = _customerModelFactory.PrepareChangePasswordModel();

            //display the cause of the change password 
            if (_workContext.CurrentCustomer.PasswordIsExpired())
                ModelState.AddModelError(string.Empty, ("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = ("Account.ChangePassword.Success");
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }


            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        [HttpsRequirement(SslRequirement.Yes)]
        public virtual IActionResult Avatar()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var model = new CustomerAvatarModel();
            model = _customerModelFactory.PrepareCustomerAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("upload-avatar")]
        public virtual IActionResult UploadAvatar(CustomerAvatarModel model, IFormFile uploadedFile)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                try
                {
                    var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
                    if (uploadedFile != null && !String.IsNullOrEmpty(uploadedFile.FileName))
                    {
                        int avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.Length > avatarMaxSize)
                            throw new NopException(string.Format(("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        byte[] customerPictureBinary = uploadedFile.GetPictureBits();
                        if (customerAvatar != null)
                            customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
                        else
                            customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, null);
                    }

                    int customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, customerAvatarId);

                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }


            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [PublicAntiForgery]
        [FormValueRequired("remove-avatar")]
        public virtual IActionResult RemoveAvatar(CustomerAvatarModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new UnauthorizedResult();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var customerAvatar = _pictureService.GetPictureById(customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId));
            if (customerAvatar != null)
                _pictureService.DeletePicture(customerAvatar);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AvatarPictureId, 0);

            return RedirectToRoute("CustomerAvatar");
        }

        #endregion

        #endregion
    }
}