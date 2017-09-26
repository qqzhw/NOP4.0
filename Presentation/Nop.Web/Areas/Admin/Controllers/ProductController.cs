using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Primitives;
using Nop.Web.Areas.Admin.Extensions;
using Nop.Web.Areas.Admin.Helpers;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Catalog; 
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog; 
using Nop.Core.Domain.Media; 
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers; 
using Nop.Services.ExportImport;
using Nop.Services.Helpers;

using Nop.Services.Logging;
using Nop.Services.Media;

using Nop.Services.Security;
using Nop.Services.Seo;
 
using Nop.Services.Stores;
 
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Models.Directory;

namespace Nop.Web.Areas.Admin.Controllers
{
    public partial class ProductController : BaseAdminController
    {
        #region Fields

        private readonly IProductService _productService; 
        private readonly ICategoryService _categoryService; 
        private readonly ICustomerService _customerService; 
        private readonly IWorkContext _workContext; 
        private readonly IPictureService _pictureService; 
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreService _storeService; 
        private readonly IStoreMappingService _storeMappingService;         
        private readonly IStaticCacheManager _cacheManager;        
        private readonly IDownloadService _downloadService;
        private readonly ISettingService _settingService;
        
        #endregion

        #region Constructors

        public ProductController(IProductService productService, 
            ICategoryService categoryService, 
            ICustomerService customerService, 
            IWorkContext workContext, 
            IPictureService pictureService, 
            ICustomerActivityService customerActivityService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreService storeService, 
            IStoreMappingService storeMappingService, 
            IStaticCacheManager cacheManager,         
            IDownloadService downloadService,
            ISettingService settingService )
        {
            this._productService = productService; 
            this._categoryService = categoryService; 
            this._customerService = customerService; 
            this._workContext = workContext; 
            this._pictureService = pictureService; 
            this._customerActivityService = customerActivityService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeService = storeService;
          
            this._storeMappingService = storeMappingService;
         
            this._cacheManager = cacheManager;
          
            this._downloadService = downloadService;
            this._settingService = settingService;
        
        }

        #endregion

        #region Utilities
         
        protected virtual void PrepareCategoryMappingModel(ProductModel model, Product product, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (!excludeProperties && product != null)
                model.SelectedCategoryIds = _categoryService.GetProductCategoriesByProductId(product.Id, true).Select(c => c.CategoryId).ToList();

            var allCategories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in allCategories)
            {
                c.Selected = model.SelectedCategoryIds.Contains(int.Parse(c.Value));
                model.AvailableCategories.Add(c);
            }
        }

        protected virtual void SaveCategoryMappings(Product product, ProductModel model)
        {
            var existingProductCategories = _categoryService.GetProductCategoriesByProductId(product.Id, true);

            //delete categories
            foreach (var existingProductCategory in existingProductCategories)
                if (!model.SelectedCategoryIds.Contains(existingProductCategory.CategoryId))
                    _categoryService.DeleteProductCategory(existingProductCategory);

            //add categories
            foreach (var categoryId in model.SelectedCategoryIds)
                if (existingProductCategories.FindProductCategory(product.Id, categoryId) == null)
                {
                    //find next display order
                    var displayOrder = 1;
                    var existingCategoryMapping = _categoryService.GetProductCategoriesByCategoryId(categoryId, showHidden: true);
                    if (existingCategoryMapping.Any())
                        displayOrder = existingCategoryMapping.Max(x => x.DisplayOrder) + 1;
                    _categoryService.InsertProductCategory(new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = displayOrder
                    });
                }
        }

       
       
     
        protected virtual string[] ParseProductTags(string productTags)
        {
            var result = new List<string>();
            if (!String.IsNullOrWhiteSpace(productTags))
            {
                string[] values = productTags.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string val1 in values)
                    if (!String.IsNullOrEmpty(val1.Trim()))
                        result.Add(val1.Trim());
            }
            return result.ToArray();
        }
        
        protected virtual void PrepareProductModel(ProductModel model, Product product, bool setPredefinedValues, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (product != null)
            {
                  
                model.CreatedOn = product.CreatedOn;
                model.UpdatedWriteOn = product.UpdatedWriteOn;
            }
            
            //supported product types
            foreach (var productType in ProductType.PCIE1.ToSelectList(false).ToList())
            {
                var productTypeId = int.Parse(productType.Value);
               
            }



            //default values

            model.Published = false;

            //editor settings 
           // model.ProductEditorSettingsModel = productEditorSettings.ToModel();
        }

        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }
 
        #endregion

        #region Methods

        #region Product list / create / edit / delete

        //list products
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var model = new ProductListModel();
            
            //categories
            model.AvailableDrivers.Add(new SelectListItem { Text = "选择磁盘", Value = "0" });
            DriveInfo[] drives = DriveInfo.GetDrives();
            CommonHelper.UploadFilePath = drives[0].Name;
            // model.AvailableDrivers.Add(new SelectListItem { Text = ("Admin.Common.All"), Value = "0" });
            foreach (var item in drives)
                model.AvailableDrivers.Add(new SelectListItem { Text = item.Name, Value = item.Name });

            //大小
            model.AvailableDMA = DmaList();

            model.AvailableMethod.Add(new SelectListItem
            {
                Text = "存盘",
                Value = "1"
            });
            model.AvailableMethod.Add(new SelectListItem
            {
                Text = "测速",
                Value = "2"
            });
            
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProductList(DataSourceRequest command, ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();
          
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
          
            var products = _productService.SearchProducts(  
                vendorId: 0, 
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true                  
            );
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x =>
                {
                    var productModel = x.ToModel();
                //little performance optimization: ensure that "FullDescription" is not returned
                productModel.Remark = "";

              
                return productModel;
                }),
                Total = products.TotalCount
            };

            return Json(gridModel);
        }

         private List<SelectListItem> DmaList()
        {
            var list = new List<SelectListItem>()
            {
                new SelectListItem()
                {
                    Text="8K",
                    Value="8",
                }, new SelectListItem()
                {
                    Text="16K",
                    Value="16",
                }, new SelectListItem()
                {
                    Text="32K",
                    Value="32",
                },
                 new SelectListItem()
                {
                    Text="64K",
                    Value="64",
                },
                  new SelectListItem()
                {
                    Text="128K",
                    Value="128",
                },
                   new SelectListItem()
                {
                    Text="256K",
                    Value="256",
                },
                new SelectListItem()
                {
                    Text="512K",
                    Value="512",
                },
                 new SelectListItem()
                {
                    Text="1024K",
                    Value="1024",
                },
                  new SelectListItem()
                {
                    Text="2048K",
                    Value="2048",
                }

            };
            return list;
            
        }
        //create product
        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            

            var model = new ProductModel();

            PrepareProductModel(model, null, true, true);
            
            PrepareCategoryMappingModel(model, null, false);
         
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

         
            if (ModelState.IsValid)
            {
             

                //product
                var product = model.ToEntity();
                product.CreatedOn = DateTime.Now;
                product.UpdatedWriteOn = DateTime.Now;
                _productService.InsertProduct(product);
              
                //categories
                SaveCategoryMappings(product, model);
          
               
                //activity log
                _customerActivityService.InsertActivity("AddNewProduct", ("ActivityLog.AddNewProduct"), product.Name);

                SuccessNotification(("Admin.Catalog.Products.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, null, false, true);
            
            PrepareCategoryMappingModel(model, null, true);
        
            return View(model);
        }

        //edit product
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

          

            var model = product.ToModel();
            PrepareProductModel(model, product, false, false);
            
            PrepareCategoryMappingModel(model, product, false);
          
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(ProductModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.Id);

            if (product == null || product.Deleted)
                //No product found with the specified id
                return RedirectToAction("List");

       
      
            if (ModelState.IsValid)
            {
              

                //we do not validate maximum number of products per vendor when editing existing products (only during creation of new products)

              

                //product
                product = model.ToEntity(product);

                product.UpdatedWriteOn = DateTime.Now;
                _productService.UpdateProduct(product);
             
                //categories
                SaveCategoryMappings(product, model);
               
              
                //activity log
                _customerActivityService.InsertActivity("EditProduct", ("ActivityLog.EditProduct"), product.Name);

                SuccessNotification(("Admin.Catalog.Products.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = product.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareProductModel(model, product, false, true);
           
            PrepareCategoryMappingModel(model, product, true);
        
            return View(model);
        }

        //delete product
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var product = _productService.GetProductById(id);
            if (product == null)
                //No product found with the specified id
                return RedirectToAction("List");

        
            _productService.DeleteProduct(product);

            //activity log
            _customerActivityService.InsertActivity("DeleteProduct", ("ActivityLog.DeleteProduct"), product.Name);

            SuccessNotification(("Admin.Catalog.Products.Deleted"));
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (selectedIds != null)
            {
                _productService.DeleteProducts(_productService.GetProductsByIds(selectedIds.ToArray()).ToList());
            }

            return Json(new { Result = true });
        }
		
		[HttpPost]
		public virtual IActionResult ConnectDevice(string diskName)
		{
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return AccessDeniedView();

			if (!string.IsNullOrEmpty(diskName))
			{
				string Info = string.Empty;
				double Percent = 0;
				DriveInfo[] drives = DriveInfo.GetDrives();
				foreach (var drive in drives)
				{
					if (drive.Name.Contains(diskName))
					{
						Info = ByteFormatter.ToString(drive.AvailableFreeSpace) + " 可用";
						Percent = 100.0 - (int)(drive.AvailableFreeSpace * 100.0 / drive.TotalSize);
					}
				}
				return Json(new { Result = true, Percent = Percent, Info = Info });
			}

			return Json(new { Result = false });
		}


		[HttpPost]
        public virtual IActionResult  SelectedDir(string diskName)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (!string.IsNullOrEmpty(diskName))
            {
				string Info = string.Empty;
				double Percent = 0;
				DriveInfo[] drives = DriveInfo.GetDrives();
				foreach (var drive in drives)
				{
					if (drive.Name.Contains(diskName))
					{
						Info = ByteFormatter.ToString(drive.AvailableFreeSpace) + " 可用";
						 Percent = 100.0 - (int)(drive.AvailableFreeSpace * 100.0 / drive.TotalSize);
					}
				}
				return Json(new { Result = true,Percent=Percent,Info=Info });
            }

            return Json(new { Result = false });
        }

        [HttpPost]
        public virtual IActionResult OpenChannel(ProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
            var findItem = _productService.GetProductById(model.Id);
            if (findItem == null)
                return Json(new { Result = false });
            else
            {
                findItem.IsOpen = model.IsOpen;
                _productService.UpdateProduct(findItem);
                return Json(new { Result = true }); 
            } 
        }


        
        #endregion




        #region Product pictures

        public virtual IActionResult ProductPictureAdd(int pictureId, int displayOrder,
            string overrideAltAttribute, string overrideTitleAttribute,
            int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            if (pictureId == 0)
                throw new ArgumentException();

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new ArgumentException("No product found with the specified id");
 
            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                overrideAltAttribute,
                overrideTitleAttribute);

            _pictureService.SetSeoFilename(pictureId, _pictureService.GetPictureSeName(product.Name));

            _productService.InsertProductPicture(new ProductPicture
            {
                PictureId = pictureId,
                ProductId = productId,
                DisplayOrder = displayOrder,
            });

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult ProductPictureList(DataSourceRequest command, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();
             
            var productPictures = _productService.GetProductPicturesByProductId(productId);
            var productPicturesModel = productPictures
                .Select(x =>
                {
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    if (picture == null)
                        throw new Exception("Picture cannot be loaded");
                    var m = new ProductModel.ProductPictureModel
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        PictureId = x.PictureId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        DisplayOrder = x.DisplayOrder
                    };
                    return m;
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = productPicturesModel,
                Total = productPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public virtual IActionResult ProductPictureUpdate(ProductModel.ProductPictureModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(model.Id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");
                     
            var picture = _pictureService.GetPictureById(productPicture.PictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.UpdatePicture(picture.Id,
                _pictureService.LoadPictureBinary(picture),
                picture.MimeType,
                picture.SeoFilename,
                model.OverrideAltAttribute,
                model.OverrideTitleAttribute);

            productPicture.DisplayOrder = model.DisplayOrder;
            _productService.UpdateProductPicture(productPicture);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual IActionResult ProductPictureDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var productPicture = _productService.GetProductPictureById(id);
            if (productPicture == null)
                throw new ArgumentException("No product picture found with the specified id");

            var productId = productPicture.ProductId;
                      
            var pictureId = productPicture.PictureId;
            _productService.DeleteProductPicture(productPicture);

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");
            _pictureService.DeletePicture(picture);

            return new NullJsonResult();
        }

        #endregion

       

        #region Export / Import

        [HttpPost, ActionName("List")]
        [FormValueRequired("download-catalog-pdf")]
        public virtual IActionResult DownloadCatalogAsPdf(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
              
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                   // _pdfService.PrintProductsToPdf(stream, products);
                    bytes = stream.ToArray();
                }
                return File(bytes, MimeTypes.ApplicationPdf, "pdfcatalog.pdf");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

       
       
        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public virtual IActionResult ExportExcelAll(ProductListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();
             
            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
           
            try
            {
                var bytes =new byte[1024] ;// _exportManager.ExportProductsToXlsx(products);

                return File(bytes, MimeTypes.TextXlsx, "products.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

     
      
        #endregion

          
 
        #endregion
    }
}