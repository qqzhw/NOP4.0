using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public partial class ProductListModel : BaseNopModel
    {
        public ProductListModel()
        {
            AvailableDrivers = new List<SelectListItem>();
            AvailableDMA = new List<SelectListItem>();
            AvailableMethod = new List<SelectListItem>();
           
        }

        [NopResourceDisplayName("可用大小")]
        public string AvailableSize { get; set; }
        [NopResourceDisplayName("自检信息")]        
 
        public string SelfCheckInfo { get; set; }
        public string Orther { get; set; }
         public string Info { get; set; }
        [NopResourceDisplayName("百分比")]
        public double DiskPercent { get; set; }

        [NopResourceDisplayName("磁盘目录")]
        public string DriverName { get; set; }
        public IList<SelectListItem> AvailableDrivers { get; set; }

        [NopResourceDisplayName("DMA读取大小")]
        public string SelectedDma { get; set; }
        public IList<SelectListItem> AvailableDMA { get; set; }

        [NopResourceDisplayName("数据方式")]
        public int SelectedMethod { get; set; }
        public IList<SelectListItem> AvailableMethod { get; set; }
         
    }
}