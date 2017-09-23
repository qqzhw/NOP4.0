using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;

using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class OnlineCustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService; 
        private readonly CustomerSettings _customerSettings;
        private readonly IPermissionService _permissionService;
     

        #endregion

        #region Ctor

        public OnlineCustomerController(ICustomerService customerService,
                   CustomerSettings customerSettings,
            IPermissionService permissionService)
        {
            this._customerService = customerService;       
         
            this._customerSettings = customerSettings;
            this._permissionService = permissionService;          
        }

        #endregion
        
        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            var customers = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes),
                null, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x => new OnlineCustomerModel
                {
                    Id = x.Id,
                    CustomerInfo = x.IsRegistered() ? x.Email : ("Admin.Customers.Guest"),
                    LastIpAddress = _customerSettings.StoreIpAddresses ?
                        x.LastIpAddress :
                        ("Admin.Customers.OnlineCustomers.Fields.IPAddress.Disabled"),
                    Location =x.LastIpAddress,
                    LastActivityDate = (x.LastActivityDateUtc),
                   
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion
    }
}