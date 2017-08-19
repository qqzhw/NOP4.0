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

          
            if (customerSettings.CountryEnabled && 
                customerSettings.StateProvinceEnabled &&
                customerSettings.StateProvinceRequired)
            {
                RuleFor(x => x.StateProvinceId).Must((x, context) =>
                { 
                    return true;
                }).WithMessage(("Account.Fields.StateProvince.Required"));
            }
            if (customerSettings.DateOfBirthEnabled && customerSettings.DateOfBirthRequired)
            {
                //entered?
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (!dateOfBirth.HasValue)
                        return false;

                    return true;
                }).WithMessage(("Account.Fields.DateOfBirth.Required"));

                //minimum age
                RuleFor(x => x.DateOfBirthDay).Must((x, context) =>
                {
                    var dateOfBirth = x.ParseDateOfBirth();
                    if (dateOfBirth.HasValue && customerSettings.DateOfBirthMinimumAge.HasValue &&
                        CommonHelper.GetDifferenceInYears(dateOfBirth.Value, DateTime.Today) <
                        customerSettings.DateOfBirthMinimumAge.Value)
                        return false;

                    return true;
                }).WithMessage(string.Format(("Account.Fields.DateOfBirth.MinimumAge"), customerSettings.DateOfBirthMinimumAge));
            }
             
         
            if (customerSettings.CityRequired && customerSettings.CityEnabled)
            {
                RuleFor(x => x.City).NotEmpty().WithMessage(("Account.Fields.City.Required"));
            }
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone).NotEmpty().WithMessage(("Account.Fields.Phone.Required"));
            }
            if (customerSettings.FaxRequired && customerSettings.FaxEnabled)
            {
                RuleFor(x => x.Fax).NotEmpty().WithMessage(("Account.Fields.Fax.Required"));
            }
        }
    }
}