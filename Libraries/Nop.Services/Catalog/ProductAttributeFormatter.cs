using System;
using System.Net;
using System.Text;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Html;


using Nop.Services.Media;


namespace Nop.Services.Catalog
{
    /// <summary>
    /// Product attribute formatter
    /// </summary>
    public partial class ProductAttributeFormatter : IProductAttributeFormatter
    {
        private readonly IWorkContext _workContext;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IProductAttributeParser _productAttributeParser;     
        private readonly IDownloadService _downloadService;
        private readonly IWebHelper _webHelper;       
        public ProductAttributeFormatter(IWorkContext workContext,
            IProductAttributeService productAttributeService,
            IProductAttributeParser productAttributeParser,        
            IDownloadService downloadService,
            IWebHelper webHelper)
        {
            this._workContext = workContext;
            this._productAttributeService = productAttributeService;
            this._productAttributeParser = productAttributeParser;         
            this._downloadService = downloadService;
            this._webHelper = webHelper;     
        }

        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(Product product, string attributesXml)
        {
            var customer = _workContext.CurrentCustomer;
            return FormatAttributes(product, attributesXml, customer);
        }
        
        /// <summary>
        /// Formats attributes
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="customer">Customer</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <param name="renderPrices">A value indicating whether to render prices</param>
        /// <param name="renderProductAttributes">A value indicating whether to render product attributes</param>
        /// <param name="renderGiftCardAttributes">A value indicating whether to render gift card attributes</param>
        /// <param name="allowHyperlinks">A value indicating whether to HTML hyperink tags could be rendered (if required)</param>
        /// <returns>Attributes</returns>
        public virtual string FormatAttributes(Product product, string attributesXml,
            Customer customer, string separator = "<br />", bool htmlEncode = true, bool renderPrices = true,
            bool renderProductAttributes = true, bool renderGiftCardAttributes = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();

            //attributes
            if (renderProductAttributes)
            {
                foreach (var attribute in _productAttributeParser.ParseProductAttributeMappings(attributesXml))
                {
                    //attributes without values
                    if (!attribute.ShouldHaveValues())
                    {
                        foreach (var value in _productAttributeParser.ParseValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Empty;
                            if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                            {
                                //multiline textbox
                                var attributeName = attribute.ProductAttribute.Name;

                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);

                                //we never encode multiline textbox input
                                formattedAttribute = string.Format("{0}: {1}", attributeName, HtmlHelper.FormatText(value, false, true, false, false, false, false));
                            }
                            else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                            {
                                //file upload
                                Guid downloadGuid;
                                Guid.TryParse(value, out downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                {
                                    var fileName = string.Format("{0}{1}", download.Filename ?? download.DownloadGuid.ToString(), download.Extension);

                                    //encode (if required)
                                    if (htmlEncode)
                                        fileName = WebUtility.HtmlEncode(fileName);

                                    //TODO add a method for getting URL (use routing because it handles all SEO friendly URLs)
                                    var attributeText = allowHyperlinks ? string.Format("<a href=\"{0}download/getfileupload/?downloadId={1}\" class=\"fileuploadattribute\">{2}</a>",
                                        _webHelper.GetStoreLocation(false), download.DownloadGuid, fileName) : fileName;

                                    var attributeName = attribute.ProductAttribute.Name;

                                    //encode (if required)
                                    if (htmlEncode)
                                        attributeName = WebUtility.HtmlEncode(attributeName);

                                    formattedAttribute = string.Format("{0}: {1}", attributeName, attributeText);
                                }
                            }
                            else
                            {
                                //other attributes (textbox, datepicker)
                                formattedAttribute = string.Format("{0}: {1}", attribute.ProductAttribute.Name, value);

                                //encode (if required)
                                if (htmlEncode)
                                    formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                            }

                            if (!string.IsNullOrEmpty(formattedAttribute))
                            {
                                if (result.Length > 0)
                                    result.Append(separator);
                                result.Append(formattedAttribute);
                            }
                        }
                    }
                    //product attribute values
                    else
                    {
                        foreach (var attributeValue in _productAttributeParser.ParseProductAttributeValues(attributesXml, attribute.Id))
                        {
                            var formattedAttribute = string.Format("{0}: {1}",
                                attribute.ProductAttribute.Name,
                                attributeValue.Name);

                            if (renderPrices)
                            {
                                decimal taxRate;
                              
                            }

                     
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);

                            if (!string.IsNullOrEmpty(formattedAttribute))
                            {
                                if (result.Length > 0)
                                    result.Append(separator);
                                result.Append(formattedAttribute);
                            }
                        }
                    }
                }
            }

            //gift cards
            if (renderGiftCardAttributes)
            {
                if (product.IsGiftCard)
                {
                    string giftCardRecipientName;
                    string giftCardRecipientEmail;
                    string giftCardSenderName;
                    string giftCardSenderEmail;
                    string giftCardMessage;
                    _productAttributeParser.GetGiftCardAttribute(attributesXml, out giftCardRecipientName, out giftCardRecipientEmail,
                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);
                                     
                  
                    if (!String.IsNullOrEmpty(result.ToString()))
                    {
                        result.Append(separator);
                    }                
                    result.Append(separator);
                
                }
            }
            return result.ToString();
        }
    }
}
