using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    public class DirectoryListModel
    {
        public DirectoryListModel()
        {
            AvailableDrivers = new List<SelectListItem>();
        }

        [NopResourceDisplayName("本地磁盘")]
        public string SearchDriverName { get; set; }

        [NopResourceDisplayName("本地磁盘")]
        public string SearchDriverId { get; set; }
        public IList<SelectListItem> AvailableDrivers { get; set; }
    }
}
