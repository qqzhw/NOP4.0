﻿using Nop.Core;
using Nop.Core.Infrastructure;

using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Themes;

namespace Nop.Web.Framework.Mvc.Razor
{
    /// <summary>
    /// Web view page
    /// </summary>
    /// <typeparam name="TModel">Model</typeparam>
    public abstract class NopRazorPage<TModel> : Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        private Localizer _localizer;

        /// <summary>
        /// Get a localized resources
        /// </summary>
        public Localizer T
        {
            get
            {
              
                if (_localizer == null)
                {
                    _localizer = (format, args) =>
                    {
                        var resFormat = (format);
                        if (string.IsNullOrEmpty(resFormat))
                        {
                            return new LocalizedString(format);
                        }
                        return new LocalizedString((args == null || args.Length == 0)
                            ? resFormat
                            : string.Format(resFormat, args));
                    };
                }
                return _localizer;
            }
        }
    }

    /// <summary>
    /// Web view page
    /// </summary>
    public abstract class NopRazorPage : NopRazorPage<dynamic>
    {
    }
}