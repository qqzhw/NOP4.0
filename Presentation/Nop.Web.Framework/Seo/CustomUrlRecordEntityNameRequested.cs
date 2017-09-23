using Microsoft.AspNetCore.Routing;
using Nop.Services.Seo;

namespace Nop.Web.Framework.Seo
{
    /// <summary>
    /// Represents event to handle unknow URL record entity names
    /// </summary>
    public class CustomUrlRecordEntityNameRequested
    {
        #region Properties

        /// <summary>
        /// Gets or sets information about the current routing path
        /// </summary>
        public RouteData RouteData { get; private set; }

       
        #endregion

        #region Ctor
         
        #endregion
    }
}