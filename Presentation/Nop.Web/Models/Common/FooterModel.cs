using System.Collections.Generic;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Models.Common
{
    public partial class FooterModel : BaseNopModel
    {
        public FooterModel()
        {
          
        }

        public string StoreName { get; set; }
 
   
        public bool SitemapEnabled { get; set; }
    
   
        public bool NewProductsEnabled { get; set; }
       
        public bool HidePoweredByNopCommerce { get; set; }

       
    }
}