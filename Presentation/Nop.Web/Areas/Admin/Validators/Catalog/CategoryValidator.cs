﻿using FluentValidation;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Catalog
{
    public partial class CategoryValidator : BaseNopValidator<CategoryModel>
    {
        public CategoryValidator(IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(("Admin.Catalog.Categories.Fields.Name.Required"));
            RuleFor(x => x.PageSizeOptions).Must(ValidatorUtilities.PageSizeOptionsValidator).WithMessage(("Admin.Catalog.Categories.Fields.PageSizeOptions.ShouldHaveUniqueItems"));
            RuleFor(x => x.PageSize).Must((x, context) =>
            {
                if (!x.AllowCustomersToSelectPageSize && x.PageSize <= 0)
                    return false;

                return true;
            }).WithMessage(("Admin.Catalog.Categories.Fields.PageSize.Positive"));

            SetDatabaseValidationRules<Category>(dbContext);
        }
    }
}