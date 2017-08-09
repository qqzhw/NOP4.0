﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Web.Framework.Themes
{
    public class ThemeableViewLocationExpander : IViewLocationExpander
    {
        private const string THEME_KEY = "nop.themename";

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            var themeContext = (IThemeContext)context.ActionContext.HttpContext.RequestServices.GetService(typeof(IThemeContext));
            context.Values[THEME_KEY] = themeContext.WorkingThemeName;
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            string theme = null;
            if (context.Values.TryGetValue(THEME_KEY, out theme))
            {
                viewLocations = new[] {
                        $"/Themes/{theme}/Views/{{1}}/{{0}}.cshtml",
                        $"/Themes/{theme}/Views/Shared/{{0}}.cshtml",
                    }
                    .Concat(viewLocations);
            }


            return viewLocations;
        }
    }
}
