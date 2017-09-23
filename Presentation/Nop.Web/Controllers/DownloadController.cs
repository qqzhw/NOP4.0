using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Catalog;

using Nop.Services.Media;


namespace Nop.Web.Controllers
{
    public partial class DownloadController : BasePublicController
    {
        private readonly IDownloadService _downloadService;
        private readonly IProductService _productService;       
        private readonly IWorkContext _workContext;     
        private readonly CustomerSettings _customerSettings;

        public DownloadController(IDownloadService downloadService,
            IProductService productService, 
            IWorkContext workContext, 
            CustomerSettings customerSettings)
        {
            this._downloadService = downloadService;
            this._productService = productService; 
            this._workContext = workContext; 
            this._customerSettings = customerSettings;
        }
        
        
             
        public virtual IActionResult GetFileUpload(Guid downloadId)
        {
            var download = _downloadService.GetDownloadByGuid(downloadId);
            if (download == null)
                return Content("Download is not available any more.");

            if (download.UseDownloadUrl)
                return new RedirectResult(download.DownloadUrl);

            //binary download
            if (download.DownloadBinary == null)
                return Content("Download data is not available any more.");

            //return result
            string fileName = !String.IsNullOrWhiteSpace(download.Filename) ? download.Filename : downloadId.ToString();
            string contentType = !String.IsNullOrWhiteSpace(download.ContentType) ? download.ContentType : MimeTypes.ApplicationOctetStream;
            return new FileContentResult(download.DownloadBinary, contentType) { FileDownloadName = fileName + download.Extension };
        }
         
    }
}