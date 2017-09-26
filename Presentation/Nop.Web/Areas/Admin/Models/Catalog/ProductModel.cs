using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Settings;
using Nop.Web.Areas.Admin.Validators.Catalog;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Web.Areas.Admin.Models.Catalog
{
    [Validator(typeof(ProductValidator))]
    public partial class ProductModel : BaseNopEntityModel
    {
        public ProductModel()
        {           
            ProductPictureModels = new List<ProductPictureModel>();
          
            AddPictureModel = new ProductPictureModel();           
             
            SelectedCategoryIds = new List<int>();
            AvailableCategories = new List<SelectListItem>(); 
        }

        [NopResourceDisplayName("ID")]
        public override int Id { get; set; }
         
        [NopResourceDisplayName("板卡ID")]
        public int ProductTypeId { get; set; }
       
    
        [NopResourceDisplayName("名称")]
        public string Name { get; set; }

        [NopResourceDisplayName("简短描述")]
        public string ShortDescription { get; set; }

        [NopResourceDisplayName("备注")]
        public string Remark { get; set; }
         
        [NopResourceDisplayName("设备ID")]
        public string DeviceId { get; set; }
   
        public bool IsOpen { get; set; }
            
         
        public bool DisableButton { get; set; }
         
        [NopResourceDisplayName("显示顺序")]
        public int DisplayOrder { get; set; }

        [NopResourceDisplayName("开关")]
        public bool Published { get; set; }

        [NopResourceDisplayName("创建时间")]
        public DateTime? CreatedOn { get; set; }
        [NopResourceDisplayName("写入时间")]
        public DateTime? UpdatedWriteOn { get; set; }
		public string  UpdatedWriteOnText { get; set; }
		[NopResourceDisplayName("VerdorId")]
        public string VendorId { get; set; }

        [NopResourceDisplayName("默认文件夹")]
        public string DefaultDir { get; set; }
        [NopResourceDisplayName("寄存器地址")]
        public int RegAddress { get; set; }

        //categories
        [NopResourceDisplayName("类型")]
        public IList<int> SelectedCategoryIds { get; set; }
        public IList<SelectListItem> AvailableCategories { get; set; }

    
        //pictures
        public ProductPictureModel AddPictureModel { get; set; }
        public IList<ProductPictureModel> ProductPictureModels { get; set; }
             
        #region Nested classes

       
        public partial class ProductPictureModel : BaseNopEntityModel
        {
            public int ProductId { get; set; }

            [UIHint("Picture")]
            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public int PictureId { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.Picture")]
            public string PictureUrl { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.DisplayOrder")]
            public int DisplayOrder { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideAltAttribute")]
            public string OverrideAltAttribute { get; set; }

            [NopResourceDisplayName("Admin.Catalog.Products.Pictures.Fields.OverrideTitleAttribute")]
            public string OverrideTitleAttribute { get; set; }
        }
          
        #endregion
    }
      
}