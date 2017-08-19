using FluentValidation;
using Nop.Web.Areas.Admin.Models.Tasks;

using Nop.Web.Framework.Validators;

namespace Nop.Web.Areas.Admin.Validators.Tasks
{
    public partial class ScheduleTaskValidator : BaseNopValidator<ScheduleTaskModel>
    {
        public ScheduleTaskValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(("Admin.System.ScheduleTasks.Name.Required"));
            RuleFor(x => x.Seconds).GreaterThan(0).WithMessage(("Admin.System.ScheduleTasks.Seconds.Positive"));
        }
    }
}