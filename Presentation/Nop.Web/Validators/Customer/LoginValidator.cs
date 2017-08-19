using FluentValidation;
using Nop.Core.Domain.Customers;

using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;

namespace Nop.Web.Validators.Customer
{
    public partial class LoginValidator : BaseNopValidator<LoginModel>
    {
        public LoginValidator(CustomerSettings customerSettings)
        {
            if (!customerSettings.UsernamesEnabled)
            {
                //login by email
                RuleFor(x => x.Email).NotEmpty().WithMessage(("Account.Login.Fields.Email.Required"));
                RuleFor(x => x.Email).EmailAddress().WithMessage(("Common.WrongEmail"));
            }
        }
    }
}