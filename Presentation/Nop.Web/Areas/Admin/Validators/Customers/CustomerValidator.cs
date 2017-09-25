using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers; 

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Customers
{
    public partial class CustomerValidator : BaseNopValidator<CustomerModel>
    {
        public CustomerValidator(ICustomerService customerService,
            CustomerSettings customerSettings,
            IDbContext dbContext)
        {
            //ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                //.WithMessage("Valid Email is required for customer to be in 'Registered' role")
                .WithMessage(("Admin.Common.WrongEmail"))
                //only for registered users
                .When(x => IsRegisteredCustomerRoleChecked(x, customerService));

         
           
            if (customerSettings.PhoneRequired && customerSettings.PhoneEnabled)
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .WithMessage(("Admin.Customers.Customers.Fields.Phone.Required"))
                    //only for registered users
                    .When(x => IsRegisteredCustomerRoleChecked(x, customerService));
            }
            
            SetDatabaseValidationRules<Customer>(dbContext);
        }

        private bool IsRegisteredCustomerRoleChecked(CustomerModel model, ICustomerService customerService)
        {
            var allCustomerRoles = customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            bool isInRegisteredRole = newCustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Registered) != null;
            return isInRegisteredRole;
        }
    }
}