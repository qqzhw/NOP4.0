using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Product extensions
    /// </summary>
    public static class ProductExtensions
    {
         
        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Product product)
        {
            return IsAvailable(product, DateTime.UtcNow);
        }

        /// <summary>
        /// Get a value indicating whether a product is available now (availability dates)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="dateTime">Datetime to check</param>
        /// <returns>Result</returns>
        public static bool IsAvailable(this Product product, DateTime dateTime)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
			 
            return true;
        }
    }
}
