using System;
using System.Linq;
using System.Net;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Services.Configuration;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Areas.Admin.Models.Home;
using Nop.Web.Framework.Mvc.Rss;
using System.IO;
using Nop.Web.Models.Directory;
using System.Collections.Generic;

namespace Nop.Web.Areas.Admin.Components
{
    public class NopCommerceNewsViewComponent : ViewComponent
    {
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly IStoreContext _storeContext;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;

        public NopCommerceNewsViewComponent(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings,
            ISettingService settingService,
            IStaticCacheManager cacheManager,
            IWebHelper webHelper)
        {
            this._storeContext = storeContext;
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
        }

        public IViewComponentResult Invoke()
        {
            try
            {
                var models = new List<DriveInfoModel>();
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (var item in drives)
                {
                    var drive = new DriveInfoModel()
                    {
                        AvailableFreeSpace = item.AvailableFreeSpace,
                        AvailableFreeSpaceText = ByteFormatter.ToString(item.AvailableFreeSpace),
                        DriveFormat = item.DriveFormat,
                        DriveType = item.DriveType,
                        IsReady = item.IsReady,
                        Name = item.Name,
                        RootDirectory = item.RootDirectory,
                        TotalFreeSpace = item.TotalFreeSpace,
                        TotalFreeSpaceText = ByteFormatter.ToString(item.TotalFreeSpace),
                        TotalSize = item.TotalSize,
                        TotalSizeText = ByteFormatter.ToString(item.TotalSize),
                        VolumeLabel = item.VolumeLabel,
                        Percent=(int)(item.AvailableFreeSpace*100.0/item.TotalSize)
                    };
                    models.Add(drive);
                }
                var model = new NopCommerceNewsModel
                {
                    HideAdvertisements = _adminAreaSettings.HideAdvertisementsOnAdminArea,
                    Items = models
                    
                };
                 
                return View(model);
            }
            catch
            {
                return Content("");
            }
        }
    }
}
