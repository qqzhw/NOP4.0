using FluentValidation;
using Nop.Web.Areas.Admin.Models.Plugins;

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Plugins
{
    public partial class PluginValidator : BaseNopValidator<PluginModel>
    {
        public PluginValidator()
        {
            RuleFor(x => x.FriendlyName).NotEmpty().WithMessage(("Admin.Configuration.Plugins.Fields.FriendlyName.Required"));
        }
    }
}