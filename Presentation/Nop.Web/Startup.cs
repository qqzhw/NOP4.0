using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Web.Framework.Infrastructure.Extensions;
using System.IO;
using Nop.Web.Models.Directory;
using System.Collections.Generic; 
 
namespace Nop.Web
{
    /// <summary>
    /// Represents startup class of application
    /// </summary>
    public class Startup
    {
        #region Properties

        /// <summary>
        /// Get configuration root of the application
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        #endregion

        #region Ctor
       
        public Startup(IHostingEnvironment environment)
        {
   //      var list = new List<DirectoryInfoModel>();
			//DriveInfo[] drives = DriveInfo.GetDrives();
   //         var d = drives[0];
   //         DirectoryInfo dir1 = new DirectoryInfo(d.Name);//声明
   //         var dirFileitems = dir1.GetFileSystemInfos();
   //         foreach (var item in dirFileitems)
   //         {
   //             if(item is DirectoryInfo)
   //             {
   //                 var directory = item as DirectoryInfo;
   //                 if ((directory.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (directory.Attributes & FileAttributes.System) != FileAttributes.System)                     
   //                 {
   //                     list.Add(new DirectoryInfoModel()
   //                     {
   //                         Root = directory.Root,
   //                         FullName=directory.FullName,
   //                          IsDir=true,
   //                          Name=directory.Name,
   //                          Parent=directory.Parent,
   //                          CreationTime=directory.CreationTime,
   //                          Exists=directory.Exists,
   //                          Extension=directory.Extension,
   //                          LastAccessTime=directory.LastAccessTime,
   //                          LastWriteTime=directory.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),                             
   //                     });
   //                 }
   //             }
   //             else if(item is FileInfo)
   //             {
   //                 var file = item as FileInfo;
   //                 if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden && (file.Attributes & FileAttributes.System) != FileAttributes.System)
   //                 {
   //                     list.Add(new DirectoryInfoModel()
   //                     {

   //                         FullName = file.FullName,
   //                         // FullPath=file.FullPath
   //                         IsDir = false,
   //                         Name = file.Name,
   //                         CreationTime = file.CreationTime,
   //                         Exists = file.Exists,
   //                         Extension = file.Extension,
   //                         LastAccessTime = file.LastAccessTime,
   //                         LastWriteTime = file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
   //                         IsReadOnly = file.IsReadOnly,
   //                         Directory = file.Directory,
   //                         DirectoryName = file.DirectoryName,
   //                         Length = file.Length,
   //                     });
   //                 }

   //             }
   //         }
                                                              //create configuration
            Configuration = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
          
        }
       
        #endregion

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.ConfigureApplicationServices(Configuration);
        }

        /// <summary>
        /// Configure the application HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            application.ConfigureRequestPipeline();
        }
    }
}
