﻿using System.Collections.Generic;
using FluentValidation.Attributes;
using Nop.Web.Areas.Admin.Validators.Customers;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    [Validator(typeof(CustomerAttributeValueValidator))]
    public partial class CustomerAttributeValueModel : BaseNopEntityModel
    {
        public CustomerAttributeValueModel()
        {
            
        }

        public int CustomerAttributeId { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.IsPreSelected")]
        public bool IsPreSelected { get; set; }

        [NopResourceDisplayName("Admin.Customers.CustomerAttributes.Values.Fields.DisplayOrder")]
        public int DisplayOrder {get;set;}

       
    }

    
}