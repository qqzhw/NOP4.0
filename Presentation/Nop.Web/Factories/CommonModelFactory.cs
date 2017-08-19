using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Media;

using Nop.Services.Security;
using Nop.Services.Seo; 
using Nop.Web.Framework.Security.Captcha;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Infrastructure.Cache; 
using Nop.Web.Models.Common;
 

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the common models factory
    /// </summary>
    public partial class CommonModelFactory : ICommonModelFactory
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
      
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ISitemapGenerator _sitemapGenerator;
        private readonly IThemeContext _themeContext;
        private readonly IThemeProvider _themeProvider;
      
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly IPageHeadBuilder _pageHeadBuilder;
        private readonly IPictureService _pictureService;
        private readonly IHostingEnvironment _hostingEnvironment; 

        private readonly CatalogSettings _catalogSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly CommonSettings _commonSettings;
       
        private readonly CaptchaSettings _captchaSettings;
      
        #endregion

        #region Constructors

        public CommonModelFactory(ICategoryService categoryService,
            IProductService productService, 
            IWorkContext workContext,
            IStoreContext storeContext,
            ISitemapGenerator sitemapGenerator,
            IThemeContext themeContext,
            IThemeProvider themeProvider, 
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            IPermissionService permissionService,
            IStaticCacheManager cacheManager,
            IPageHeadBuilder pageHeadBuilder,
            IPictureService pictureService,
            IHostingEnvironment hostingEnvironment,
            CatalogSettings catalogSettings,
            StoreInformationSettings storeInformationSettings,
            CommonSettings commonSettings, 
            CaptchaSettings captchaSettings )
        {
            this._categoryService = categoryService;
            this._productService = productService;
           
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._sitemapGenerator = sitemapGenerator;
            this._themeContext = themeContext;
            this._themeProvider = themeProvider;
          
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
            this._permissionService = permissionService;
            this._cacheManager = cacheManager;
            this._pageHeadBuilder = pageHeadBuilder;
            this._pictureService = pictureService;
            this._hostingEnvironment = hostingEnvironment;
            this._catalogSettings = catalogSettings;
            this._storeInformationSettings = storeInformationSettings;
            this._commonSettings = commonSettings;
      
            this._captchaSettings = captchaSettings;
          
        }

        #endregion

        
        #region Methods

        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        public virtual LogoModel PrepareLogoModel()
        {
            var model = new LogoModel
            {
                StoreName = _storeContext.CurrentStore.Name
            };

            var cacheKey = string.Format(ModelCacheEventConsumer.STORE_LOGO_PATH, _storeContext.CurrentStore.Id, _themeContext.WorkingThemeName, _webHelper.IsCurrentConnectionSecured());
            model.LogoPath = _cacheManager.Get(cacheKey, () =>
            {
                var logo = "";
                var logoPictureId = _storeInformationSettings.LogoPictureId;
                if (logoPictureId > 0)
                {
                    logo = _pictureService.GetPictureUrl(logoPictureId, showDefaultPicture: false);
                }
                if (String.IsNullOrEmpty(logo))
                {
                    //use default logo
                    logo = string.Format("{0}Themes/{1}/Content/images/logo.png", _webHelper.GetStoreLocation(), _themeContext.WorkingThemeName);
                }
                return logo;
            });

            return model;
        }

      
        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        public virtual HeaderLinksModel PrepareHeaderLinksModel()
        {
            var customer = _workContext.CurrentCustomer;
          
            var unreadMessage = string.Empty;
            var alertMessage = string.Empty;          
            var model = new HeaderLinksModel
            {
                IsAuthenticated = customer.IsRegistered(),
                CustomerName = customer.IsRegistered() ? customer.FormatUserName() : "",
                ShoppingCartEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart),
                WishlistEnabled = _permissionService.Authorize(StandardPermissionProvider.EnableWishlist),
                AllowPrivateMessages = customer.IsRegistered() ,
                UnreadPrivateMessages = unreadMessage,
                AlertMessage = alertMessage,
            };
           
            return model;
        }

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        public virtual AdminHeaderLinksModel PrepareAdminHeaderLinksModel()
        {
            var customer = _workContext.CurrentCustomer;

            var model = new AdminHeaderLinksModel
            {
                ImpersonatedCustomerName = customer.IsRegistered() ? customer.FormatUserName() : "",
                IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
                DisplayAdminLink = _permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel),
                EditPageUrl = _pageHeadBuilder.GetEditPageUrl()
            };

            return model;
        }

       
        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        public virtual FooterModel PrepareFooterModel()
        {
            //footer topics
            string topicCacheKey = string.Format(ModelCacheEventConsumer.TOPIC_FOOTER_MODEL_KEY,
                1,
                _storeContext.CurrentStore.Id,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()) );

            //model
            var model = new FooterModel
            {
                StoreName = _storeContext.CurrentStore.Name,
             
                SitemapEnabled = _commonSettings.SitemapEnabled,
              
                NewProductsEnabled = _catalogSettings.NewProductsEnabled,
             
                HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
          
            };

            return model;
        }
 
      
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="url">URL helper</param>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        public virtual string PrepareSitemapXml(IUrlHelper url, int? id)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.SITEMAP_SEO_MODEL_KEY, id,
               1,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            var siteMap = _cacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(url, id));
            return siteMap;
        }

       
        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        public virtual FaviconModel PrepareFaviconModel()
        {
            var model = new FaviconModel();

            //try loading a store specific favicon

            var faviconFileName = string.Format("favicon-{0}.ico", _storeContext.CurrentStore.Id);
            var localFaviconPath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, faviconFileName);
            if (!System.IO.File.Exists(localFaviconPath))
            {
                //try loading a generic favicon
                faviconFileName = "favicon.ico";
                localFaviconPath = System.IO.Path.Combine(_hostingEnvironment.WebRootPath, faviconFileName);
                if (!System.IO.File.Exists(localFaviconPath))
                {
                    return model;
                }
            }

            model.FaviconUrl = _webHelper.GetStoreLocation() + faviconFileName;
            return model;
        }

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        public virtual string PrepareRobotsTextFile()
        {
            var sb = new StringBuilder();

            //if robots.custom.txt exists, let's use it instead of hard-coded data below
            string robotsFilePath = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.custom.txt");
            if (System.IO.File.Exists(robotsFilePath))
            {
                //the robots.txt file exists
                string robotsFileContent = System.IO.File.ReadAllText(robotsFilePath);
                sb.Append(robotsFileContent);
            }
            else
            {
                //doesn't exist. Let's generate it (default behavior)

                var disallowPaths = new List<string>
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/setproductreviewhelpfulness",
                };
                var localizableDisallowPaths = new List<string>
                {                    
                    "/customer/changepassword",
                    "/customer/checkusernameavailability",
                    "/customer/downloadableproducts",
                    "/customer/info",
                    "/deletepm",                
                    "/inboxupdate", 
                    "/passwordrecovery/confirm", 
                    "/sentupdate",                 
                    "/storeclosed", 
                    "/uploadfilecheckoutattribute",
                    "/uploadfileproductattribute",
                    "/uploadfilereturnrequest",
          
                };


                const string newLine = "\r\n"; //Environment.NewLine
                sb.Append("User-agent: *");
                sb.Append(newLine);
                //sitemaps
               
                {
                    //localizable paths (without SEO code)
                    sb.AppendFormat("Sitemap: {0}sitemap.xml", _storeContext.CurrentStore.Url);
                    sb.Append(newLine);
                }

                //usual paths
                foreach (var path in disallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                //localizable paths (without SEO code)
                foreach (var path in localizableDisallowPaths)
                {
                    sb.AppendFormat("Disallow: {0}", path);
                    sb.Append(newLine);
                }
                

                //load and add robots.txt additions to the end of file.
                string robotsAdditionsFile = System.IO.Path.Combine(CommonHelper.MapPath("~/"), "robots.additions.txt");
                if (System.IO.File.Exists(robotsAdditionsFile))
                {
                    string robotsFileContent = System.IO.File.ReadAllText(robotsAdditionsFile);
                    sb.Append(robotsFileContent);
                }
            }

            return sb.ToString();
        }
        
#endregion
    }
}
