using FluentValidation;
using Nop.Web.Areas.Admin.Models.Settings;

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Settings
{
    public partial class SettingValidator : BaseNopValidator<SettingModel>
    {
        public SettingValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(("Admin.Configuration.Settings.AllSettings.Fields.Name.Required"));
        }
    }
}