using FluentValidation;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Core.Domain.Stores;
using Nop.Data;

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Stores
{
    public partial class StoreValidator : BaseNopValidator<StoreModel>
    {
        public StoreValidator( IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.Url).NotEmpty().WithMessage(("Admin.Configuration.Stores.Fields.Url.Required"));

            SetDatabaseValidationRules<Store>(dbContext);
        }
    }
}