using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the customer model factory
    /// </summary>
    public partial interface ICustomerModelFactory
    {
       
        /// <summary>
        /// Prepare the customer info model
        /// </summary>
        /// <param name="model">Customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>Customer info model</returns>
        CustomerInfoModel PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer, 
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "");

        
        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        LoginModel PrepareLoginModel(bool? checkoutAsGuest);

   
        /// <summary>
        /// Prepare the customer navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>Customer navigation model</returns>
        CustomerNavigationModel PrepareCustomerNavigationModel(int selectedTabId = 0);

    

        /// <summary>
        /// Prepare the customer downloadable products model
        /// </summary>
        /// <returns>Customer downloadable products model</returns>
        CustomerDownloadableProductsModel PrepareCustomerDownloadableProductsModel();

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>User agreement model</returns>
        UserAgreementModel PrepareUserAgreementModel( Product product);

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        ChangePasswordModel PrepareChangePasswordModel();

        /// <summary>
        /// Prepare the customer avatar model
        /// </summary>
        /// <param name="model">Customer avatar model</param>
        /// <returns>Customer avatar model</returns>
       
    }
}
