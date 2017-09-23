using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Nop.Core;
using Nop.Core.Data;

using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Services.Seo;
using Nop.Web.Framework.Localization;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Provides properties and methods for defining a SEO friendly route, and for getting information about the route.
    /// </summary>
    public class GenericPathRoute : LocalizedRoute
    {
        #region Fields

        private readonly IRouter _target;

        #endregion

        #region Ctor

        public GenericPathRoute(IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, 
            IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Get route values for current route
        /// </summary>
        /// <param name="context">Route context</param>
        /// <returns>Route values</returns>
        protected RouteValueDictionary GetRouteValues(RouteContext context)
        {
            //remove language code from the path if it's localized URL
            var path = context.HttpContext.Request.Path.Value;
            //if (this.SeoFriendlyUrlsForLanguagesEnabled && path.IsLocalizedUrl(context.HttpContext.Request.PathBase, false))
            //    path = path.RemoveLanguageSeoCodeFromUrl(context.HttpContext.Request.PathBase, false);

            //parse route data
            var routeValues = new RouteValueDictionary(this.ParsedTemplate.Parameters
                .Where(parameter => parameter.DefaultValue != null)
                .ToDictionary(parameter => parameter.Name, parameter => parameter.DefaultValue));
            var matcher = new TemplateMatcher(this.ParsedTemplate, routeValues);
            matcher.TryMatch(path, routeValues);

            return routeValues;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Route request to the particular action
        /// </summary>
        /// <param name="context">A route context object</param>
        /// <returns>Task of the routing</returns>
        public override Task RouteAsync(RouteContext context)
        {
            if (!DataSettingsHelper.DatabaseIsInstalled())
                return Task.CompletedTask;

            //try to get slug from the route data
            var routeValues = GetRouteValues(context);
            if (!routeValues.TryGetValue("GenericSeName", out object slugValue) || string.IsNullOrEmpty(slugValue as string))
                return Task.CompletedTask;

            var slug = slugValue as string;

            //performance optimization, we load a cached verion here. It reduces number of SQL requests for each page load
          
             
            //comment the line above and uncomment the line below in order to disable this performance "workaround"
            //var urlRecord = urlRecordService.GetBySlug(slug);

           

            //virtual directory path
            var pathBase = context.HttpContext.Request.PathBase;

            //if URL record is not active let's find the latest one
           
            //ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when customers choose a new language but a slug stays the same
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
     
             

            //since we are here, all is ok with the slug, so process URL
            var currentRouteData = new RouteData(context.RouteData);
          
            context.RouteData = currentRouteData;

            //route request
            return _target.RouteAsync(context);
        }

        #endregion
    }
}