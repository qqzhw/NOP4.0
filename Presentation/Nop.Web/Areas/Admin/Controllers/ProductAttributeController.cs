using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;

using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;

        #endregion Fields

        #region Constructors

        public ProductAttributeController(IProductService productService,
            IProductAttributeService productAttributeService,           
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService)
        {
            this._productService = productService;
            this._productAttributeService = productAttributeService;
         
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
        }

        #endregion
        
             
        #region Methods

        #region Attribute list / create / edit / delete

        //list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public virtual IActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var productAttributes = _productAttributeService
                .GetAllProductAttributes(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = productAttributes.Select(x => x.ToModel()),
                Total = productAttributes.TotalCount
            };

            return Json(gridModel);
        }
        
        //create
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var model = new ProductAttributeModel();
          
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var productAttribute = model.ToEntity();
                _productAttributeService.InsertProductAttribute(productAttribute);
              
                //activity log
                _customerActivityService.InsertActivity("AddNewProductAttribute", ("ActivityLog.AddNewProductAttribute"), productAttribute.Name);

                SuccessNotification(("Admin.Catalog.Attributes.ProductAttributes.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = productAttribute.Id });
                }
                return RedirectToAction("List");

            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //edit
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            var model = productAttribute.ToModel();
        
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductAttributeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(model.Id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");
            
            if (ModelState.IsValid)
            {
                productAttribute = model.ToEntity(productAttribute);
                _productAttributeService.UpdateProductAttribute(productAttribute);
                
                //activity log
                _customerActivityService.InsertActivity("EditProductAttribute", ("ActivityLog.EditProductAttribute"), productAttribute.Name);

                SuccessNotification(("Admin.Catalog.Attributes.ProductAttributes.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = productAttribute.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //delete
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            var productAttribute = _productAttributeService.GetProductAttributeById(id);
            if (productAttribute == null)
                //No product attribute found with the specified id
                return RedirectToAction("List");

            _productAttributeService.DeleteProductAttribute(productAttribute);

            //activity log
            _customerActivityService.InsertActivity("DeleteProductAttribute", ("ActivityLog.DeleteProductAttribute"), productAttribute.Name);

            SuccessNotification(("Admin.Catalog.Attributes.ProductAttributes.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Used by products

        //used by products
        [HttpPost]
        public virtual IActionResult UsedByProducts(DataSourceRequest command, int productAttributeId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedKendoGridJson();

            var products = _productService.GetProductsByProductAtributeId(
                productAttributeId: productAttributeId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x =>
                {
                    return new ProductAttributeModel.UsedByProductModel
                    {
                        Id = x.Id,
                        ProductName = x.Name,
                        Published = x.Published
                    };
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }
        
        #endregion
               
        #endregion
    }
}