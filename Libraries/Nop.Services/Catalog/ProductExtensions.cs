using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class ProductExtensions
    {
         
       

      
           
        
        /// <summary>
        /// Formats start/end date for rental product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        public static string FormatRentalDate(this Product product, DateTime date)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

          

            return date.ToShortDateString();
        }
         
    }
}
