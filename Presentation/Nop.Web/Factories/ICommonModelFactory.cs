using Microsoft.AspNetCore.Mvc;
using Nop.Web.Models.Common;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the interface of the common models factory
    /// </summary>
    public partial interface ICommonModelFactory
    {
        /// <summary>
        /// Prepare the logo model
        /// </summary>
        /// <returns>Logo model</returns>
        LogoModel PrepareLogoModel();

      
        /// <summary>
        /// Prepare the header links model
        /// </summary>
        /// <returns>Header links model</returns>
        HeaderLinksModel PrepareHeaderLinksModel();

        /// <summary>
        /// Prepare the admin header links model
        /// </summary>
        /// <returns>Admin header links model</returns>
        AdminHeaderLinksModel PrepareAdminHeaderLinksModel();

      
        /// <summary>
        /// Prepare the footer model
        /// </summary>
        /// <returns>Footer model</returns>
        FooterModel PrepareFooterModel();
		 
        /// <summary>
        /// Get the sitemap in XML format
        /// </summary>
        /// <param name="url">URL helper</param>
        /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
        /// <returns>Sitemap as string in XML format</returns>
        string PrepareSitemapXml(IUrlHelper url, int? id);

      

        /// <summary>
        /// Prepare the favicon model
        /// </summary>
        /// <returns>Favicon model</returns>
        FaviconModel PrepareFaviconModel();

        /// <summary>
        /// Get robots.txt file
        /// </summary>
        /// <returns>Robots.txt file as string</returns>
        string PrepareRobotsTextFile();
    }
}
