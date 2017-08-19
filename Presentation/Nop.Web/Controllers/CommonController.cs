
using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Extensions;
using Nop.Services.Common; 
using Nop.Services.Logging; 
using Nop.Web.Factories;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Security;
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Common;

namespace Nop.Web.Controllers
{
    public partial class CommonController : BasePublicController
    {
        #region Fields

        private readonly ICommonModelFactory _commonModelFactory;    
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IThemeContext _themeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerActivityService _customerActivityService;       
        private readonly ILogger _logger;        
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CommonSettings _commonSettings;     
        private readonly CaptchaSettings _captchaSettings;
     
        #endregion
        
        #region Constructors

        public CommonController(ICommonModelFactory commonModelFactory,           
            IWorkContext workContext,
            IStoreContext storeContext,
            IThemeContext themeContext,
            IGenericAttributeService genericAttributeService,
            ICustomerActivityService customerActivityService,         
            ILogger logger,
            StoreInformationSettings storeInformationSettings,
            CommonSettings commonSettings,       
            CaptchaSettings captchaSettings)
        {
            this._commonModelFactory = commonModelFactory;         
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._themeContext = themeContext;
            this._genericAttributeService = genericAttributeService;
            this._customerActivityService = customerActivityService;
         
            this._logger = logger;
            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
       
            this._captchaSettings = captchaSettings;
          
        }

        #endregion

        #region Methods
        
        //page not found
        public virtual IActionResult PageNotFound()
        {
            if (_commonSettings.Log404Errors)
            {
                var statusCodeReExecuteFeature = HttpContext?.Features?.Get<IStatusCodeReExecuteFeature>();
                //TODO add locale resource
                _logger.Error(string.Format("Error 404. The requested page ({0}) was not found", statusCodeReExecuteFeature?.OriginalPath), 
                    customer: _workContext.CurrentCustomer);
            }

            this.Response.StatusCode = 404;
            this.Response.ContentType = "text/html";

            return View();
        }
        
          
       
        public virtual IActionResult SetStoreTheme(string themeName, string returnUrl = "")
        {
            _themeContext.WorkingThemeName = themeName;

            //home page
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                returnUrl = Url.RouteUrl("HomePage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult EuCookieLawAccept()
        {
            if (!_storeInformationSettings.DisplayEuCookieLawWarning)
                //disabled
                return Json(new { stored = false });

            //save setting
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.EuCookieLawAccepted, true, _storeContext.CurrentStore.Id);
            return Json(new { stored = true });
        }

        //robots.txt file
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult RobotsTextFile()
        {
            var robotsFileContent = _commonModelFactory.PrepareRobotsTextFile();
            return Content(robotsFileContent, MimeTypes.TextPlain);
        }

        public virtual IActionResult GenericUrl()
        {
            //seems that no entity was found
            return InvokeHttp404();
        }

        //store is closed
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult StoreClosed()
        {
            return View();
        }

        //helper method to redirect users. Workaround for GenericPathRoute class where we're not allowed to do it
        public virtual IActionResult InternalRedirect(string url, bool permanentRedirect)
        {
            //ensure it's invoked from our GenericPathRoute class
            if (HttpContext.Items["nop.RedirectFromGenericPathRoute"] == null ||
                !Convert.ToBoolean(HttpContext.Items["nop.RedirectFromGenericPathRoute"]))
            {
                url = Url.RouteUrl("HomePage");
                permanentRedirect = false;
            }

            //home page
            if (String.IsNullOrEmpty(url))
            {
                url = Url.RouteUrl("HomePage");
                permanentRedirect = false;
            }

            //prevent open redirection attack
            if (!Url.IsLocalUrl(url))
            {
                url = Url.RouteUrl("HomePage");
                permanentRedirect = false;
            }

            if (permanentRedirect)
                return RedirectPermanent(url);
            else
                return Redirect(url);
        }

        #endregion
    }
}