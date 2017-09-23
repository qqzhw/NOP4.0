using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.Services.Helpers;

using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Tasks;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ScheduleTaskController : BaseAdminController
	{
		#region Fields

        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IPermissionService _permissionService;
      
        private readonly ICustomerActivityService _customerActivityService;

        #endregion

        #region Constructors

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService, 
            IPermissionService permissionService,          
            ICustomerActivityService customerActivityService)
        {
            this._scheduleTaskService = scheduleTaskService;
            this._permissionService = permissionService;
              this._customerActivityService = customerActivityService;
        }

		#endregion 

        #region Utility

        protected virtual ScheduleTaskModel PrepareScheduleTaskModel(ScheduleTask task)
        {
            var model = new ScheduleTaskModel
                            {
                                Id = task.Id,
                                Name = task.Name,
                                Seconds = task.Seconds,
                                Enabled = task.Enabled,
                                StopOnError = task.StopOnError,
                                LastStartUtc = task.LastStartUtc.HasValue ? (task.LastStartUtc.Value).ToString("G") : "",
                                LastEndUtc = task.LastEndUtc.HasValue ?(task.LastEndUtc.Value).ToString("G") : "",
                                LastSuccessUtc = task.LastSuccessUtc.HasValue ? (task.LastSuccessUtc.Value).ToString("G") : "",
                            };
            return model;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            return View();
		}

		[HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedKendoGridJson();

            var models = _scheduleTaskService.GetAllTasks(true)
                .Select(PrepareScheduleTaskModel)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = models,
                Total = models.Count
            };

            return Json(gridModel);
		}

        [HttpPost]
        public virtual IActionResult TaskUpdate(ScheduleTaskModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            if (!ModelState.IsValid)
            {
                return Json(new DataSourceResult { Errors = ModelState.SerializeErrors() });
            }

            var scheduleTask = _scheduleTaskService.GetTaskById(model.Id);
            if (scheduleTask == null)
                return Content("Schedule task cannot be loaded");

            scheduleTask.Name = model.Name;
            scheduleTask.Seconds = model.Seconds;
            scheduleTask.Enabled = model.Enabled;
            scheduleTask.StopOnError = model.StopOnError;
            _scheduleTaskService.UpdateTask(scheduleTask);

            //activity log
            _customerActivityService.InsertActivity("EditTask", ("ActivityLog.EditTask"), scheduleTask.Id);

            return new NullJsonResult();
        }

        public virtual IActionResult RunNow(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            try
            {
                var scheduleTask = _scheduleTaskService.GetTaskById(id);
                if (scheduleTask == null)
                    throw new Exception("Schedule task cannot be loaded");
                
                var task = new Task(scheduleTask) {Enabled = true};
                //ensure that the task is enabled
                task.Execute(true, false);
                SuccessNotification(("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
            }

            return RedirectToAction("List");
        }
        #endregion
    }
}