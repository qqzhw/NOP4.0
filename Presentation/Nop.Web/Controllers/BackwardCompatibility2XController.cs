using Microsoft.AspNetCore.Mvc;
 
using Nop.Services.Catalog;
 
using Nop.Services.Seo; 
namespace Nop.Web.Controllers
{
    public partial class BackwardCompatibility2XController : BasePublicController
    {
		#region Fields

        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
     

        #endregion

		#region Constructors

        public BackwardCompatibility2XController(IProductService productService,
            ICategoryService categoryService )
        {
            this._productService = productService;
            this._categoryService = categoryService;
         
        }

		#endregion
        
        #region Methods
        
        //in versions 2.00-2.65 we had ID in product URLs
        public virtual IActionResult RedirectProductById(int productId)
        {
            var product = _productService.GetProductById(productId);
            if (product == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Product", new { SeName = product.GetSeName() });
        }
        //in versions 2.00-2.65 we had ID in category URLs
        public virtual IActionResult RedirectCategoryById(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                return RedirectToRoutePermanent("HomePage");

            return RedirectToRoutePermanent("Category", new { SeName = category.GetSeName() });
        }
            
        #endregion
    }
}