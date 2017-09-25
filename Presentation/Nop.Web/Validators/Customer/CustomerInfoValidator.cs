using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Nop.Core;
using Nop.Core.Domain.Customers; 

using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public partial class CustomerInfoValidator : BaseNopValidator<CustomerInfoModel>
    {
        public CustomerInfoValidator( CustomerSettings customerSettings)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(("Common.WrongEmail"));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(("Account.Fields.FirstName.Required"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(("Account.Fields.LastName.Required"));

            if (customerSettings.UsernamesEnabled && customerSettings.AllowUsersToChangeUsernames)
            {
                RuleFor(x => x.Username).NotEmpty().WithMessage(("Account.Fields.Username.Required"));
            }

          
         
        
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage(("Account.Fields.Phone.Required"));
            }
           
        }
    }
}