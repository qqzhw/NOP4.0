using FluentValidation;
using Nop.Web.Areas.Admin.Models.Catalog;

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class PredefinedProductAttributeValueModelValidator : BaseNopValidator<PredefinedProductAttributeValueModel>
    {
        public PredefinedProductAttributeValueModelValidator( )
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(("Admin.Catalog.Attributes.ProductAttributes.PredefinedValues.Fields.Name.Required"));
        }
    }
}