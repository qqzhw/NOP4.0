﻿using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Common;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class PreferencesController : BaseAdminController
    {
        #region Fields
        
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PreferencesController(IWorkContext workContext)
        {
          
            this._workContext = workContext;
        }

        #endregion

        #region Methods

        [HttpPost]
        public virtual IActionResult SavePreference(string name, bool value)
        {
            //permission validation is not required here
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            
            return Json(new
            {
                Result = true
            });
        }

        #endregion
    }
}