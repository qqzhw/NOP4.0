﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Nop.Web.Framework.TagHelpers.Public
{
    [HtmlTargetElement("textarea", Attributes = ForAttributeName)]
    public class TextAreaTagHelper : Microsoft.AspNetCore.Mvc.TagHelpers.TextAreaTagHelper
    {
        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// Indicates whether the input is disabled
        /// </summary>
        [HtmlAttributeName("asp-disabled")]
        public string IsDisabled { set; get; }

        public TextAreaTagHelper(IHtmlGenerator generator) : base(generator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //add disabled attribute
            bool.TryParse(IsDisabled, out bool disabled);
            if (disabled)
            {
                var d = new TagHelperAttribute("disabled", "disabled");
                output.Attributes.Add(d);
            }

            base.Process(context, output);
        }
    }
}