using System;
using System.Linq;
 
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Cms;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
 
using Nop.Web.Areas.Admin.Models.ExternalAuthentication;
 
using Nop.Web.Areas.Admin.Models.Logging;
using Nop.Web.Areas.Admin.Models.Messages;
 
using Nop.Web.Areas.Admin.Models.Plugins;
 
using Nop.Web.Areas.Admin.Models.Settings;
 
using Nop.Web.Areas.Admin.Models.Stores;
 
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Polls;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Stores;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Topics;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure.Mapper;
using Nop.Core.Plugins;
using Nop.Services.Authentication.External;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Payments;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Pickup;
using Nop.Services.Tax;
using Nop.Web.Framework.Security.Captcha;

namespace Nop.Web.Areas.Admin.Extensions
{
    public static class MappingExtensions
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }
        
        #region Category

        public static CategoryModel ToModel(this Category entity)
        {
            return entity.MapTo<Category, CategoryModel>();
        }

        public static Category ToEntity(this CategoryModel model)
        {
            return model.MapTo<CategoryModel, Category>();
        }

        public static Category ToEntity(this CategoryModel model, Category destination)
        {
            return model.MapTo(destination);
        }

        #endregion
		 

        #region Products

        public static ProductModel ToModel(this Product entity)
        {
            return entity.MapTo<Product, ProductModel>();
        }

        public static Product ToEntity(this ProductModel model)
        {
            return model.MapTo<ProductModel, Product>();
        }

        public static Product ToEntity(this ProductModel model, Product destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Product attributes

        public static ProductAttributeModel ToModel(this ProductAttribute entity)
        {
            return entity.MapTo<ProductAttribute, ProductAttributeModel>();
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model)
        {
            return model.MapTo<ProductAttributeModel, ProductAttribute>();
        }

        public static ProductAttribute ToEntity(this ProductAttributeModel model, ProductAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

       

        #region Customer attributes

        //attributes
        public static CustomerAttributeModel ToModel(this CustomerAttribute entity)
        {
            return entity.MapTo<CustomerAttribute, CustomerAttributeModel>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model)
        {
            return model.MapTo<CustomerAttributeModel, CustomerAttribute>();
        }

        public static CustomerAttribute ToEntity(this CustomerAttributeModel model, CustomerAttribute destination)
        {
            return model.MapTo(destination);
        }

        #endregion

       
		 

        #region Email account

        public static EmailAccountModel ToModel(this EmailAccount entity)
        {
            return entity.MapTo<EmailAccount, EmailAccountModel>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model)
        {
            return model.MapTo<EmailAccountModel, EmailAccount>();
        }

        public static EmailAccount ToEntity(this EmailAccountModel model, EmailAccount destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Message templates

        public static MessageTemplateModel ToModel(this MessageTemplate entity)
        {
            return entity.MapTo<MessageTemplate, MessageTemplateModel>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model)
        {
            return model.MapTo<MessageTemplateModel, MessageTemplate>();
        }

        public static MessageTemplate ToEntity(this MessageTemplateModel model, MessageTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued email

        public static QueuedEmailModel ToModel(this QueuedEmail entity)
        {
            return entity.MapTo<QueuedEmail, QueuedEmailModel>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model)
        {
            return model.MapTo<QueuedEmailModel, QueuedEmail>();
        }

        public static QueuedEmail ToEntity(this QueuedEmailModel model, QueuedEmail destination)
        {
            return model.MapTo(destination);
        }

        #endregion
		 
        #region Log

        public static LogModel ToModel(this Log entity)
        {
            return entity.MapTo<Log, LogModel>();
        }

        public static Log ToEntity(this LogModel model)
        {
            return model.MapTo<LogModel, Log>();
        }

        public static Log ToEntity(this LogModel model, Log destination)
        {
            return model.MapTo(destination);
        }

        public static ActivityLogTypeModel ToModel(this ActivityLogType entity)
        {
            return entity.MapTo<ActivityLogType, ActivityLogTypeModel>();
        }

        public static ActivityLogModel ToModel(this ActivityLog entity)
        {
            return entity.MapTo<ActivityLog, ActivityLogModel>();
        }

        #endregion
  
        #region External authentication methods

        public static AuthenticationMethodModel ToModel(this IExternalAuthenticationMethod entity)
        {
            return entity.MapTo<IExternalAuthenticationMethod, AuthenticationMethodModel>();
        }

        #endregion
        
        #region Widgets

        public static WidgetModel ToModel(this IWidgetPlugin entity)
        {
            return entity.MapTo<IWidgetPlugin, WidgetModel>();
        }

        #endregion
		 
       

        #region Customer roles

        //customer roles
        public static CustomerRoleModel ToModel(this CustomerRole entity)
        {
            return entity.MapTo<CustomerRole, CustomerRoleModel>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model)
        {
            return model.MapTo<CustomerRoleModel, CustomerRole>();
        }

        public static CustomerRole ToEntity(this CustomerRoleModel model, CustomerRole destination)
        {
            return model.MapTo(destination);
        }

        #endregion
		 

        #region Settings

        public static TaxSettingsModel ToModel(this TaxSettings entity)
        {
            return entity.MapTo<TaxSettings, TaxSettingsModel>();
        }
        public static TaxSettings ToEntity(this TaxSettingsModel model, TaxSettings destination)
        {
            return model.MapTo(destination);
        }


        public static ShippingSettingsModel ToModel(this ShippingSettings entity)
        {
            return entity.MapTo<ShippingSettings, ShippingSettingsModel>();
        }
        public static ShippingSettings ToEntity(this ShippingSettingsModel model, ShippingSettings destination)
        {
            return model.MapTo(destination);
        }


        public static ForumSettingsModel ToModel(this ForumSettings entity)
        {
            return entity.MapTo<ForumSettings, ForumSettingsModel>();
        }
        public static ForumSettings ToEntity(this ForumSettingsModel model, ForumSettings destination)
        {
            return model.MapTo(destination);
        }


        public static BlogSettingsModel ToModel(this BlogSettings entity)
        {
            return entity.MapTo<BlogSettings, BlogSettingsModel>();
        }
        public static BlogSettings ToEntity(this BlogSettingsModel model, BlogSettings destination)
        {
            return model.MapTo(destination);
        }


        public static VendorSettingsModel ToModel(this VendorSettings entity)
        {
            return entity.MapTo<VendorSettings, VendorSettingsModel>();
        }
        public static VendorSettings ToEntity(this VendorSettingsModel model, VendorSettings destination)
        {
            return model.MapTo(destination);
        }


        public static NewsSettingsModel ToModel(this NewsSettings entity)
        {
            return entity.MapTo<NewsSettings, NewsSettingsModel>();
        }
        public static NewsSettings ToEntity(this NewsSettingsModel model, NewsSettings destination)
        {
            return model.MapTo(destination);
        }


        public static CatalogSettingsModel ToModel(this CatalogSettings entity)
        {
            return entity.MapTo<CatalogSettings, CatalogSettingsModel>();
        }
        public static CatalogSettings ToEntity(this CatalogSettingsModel model, CatalogSettings destination)
        {
            return model.MapTo(destination);
        }


        public static RewardPointsSettingsModel ToModel(this RewardPointsSettings entity)
        {
            return entity.MapTo<RewardPointsSettings, RewardPointsSettingsModel>();
        }
        public static RewardPointsSettings ToEntity(this RewardPointsSettingsModel model, RewardPointsSettings destination)
        {
            return model.MapTo(destination);
        }


        public static OrderSettingsModel ToModel(this OrderSettings entity)
        {
            return entity.MapTo<OrderSettings, OrderSettingsModel>();
        }
        public static OrderSettings ToEntity(this OrderSettingsModel model, OrderSettings destination)
        {
            return model.MapTo(destination);
        }


        public static ShoppingCartSettingsModel ToModel(this ShoppingCartSettings entity)
        {
            return entity.MapTo<ShoppingCartSettings, ShoppingCartSettingsModel>();
        }
        public static ShoppingCartSettings ToEntity(this ShoppingCartSettingsModel model, ShoppingCartSettings destination)
        {
            return model.MapTo(destination);
        }


        public static MediaSettingsModel ToModel(this MediaSettings entity)
        {
            return entity.MapTo<MediaSettings, MediaSettingsModel>();
        }
        public static MediaSettings ToEntity(this MediaSettingsModel model, MediaSettings destination)
        {
            return model.MapTo(destination);
        }

        //customer/user settings
        public static CustomerUserSettingsModel.CustomerSettingsModel ToModel(this CustomerSettings entity)
        {
            return entity.MapTo<CustomerSettings, CustomerUserSettingsModel.CustomerSettingsModel>();
        }
        public static CustomerSettings ToEntity(this CustomerUserSettingsModel.CustomerSettingsModel model, CustomerSettings destination)
        {
            return model.MapTo(destination);
        }
        public static CustomerUserSettingsModel.AddressSettingsModel ToModel(this AddressSettings entity)
        {
            return entity.MapTo<AddressSettings, CustomerUserSettingsModel.AddressSettingsModel>();
        }
        public static AddressSettings ToEntity(this CustomerUserSettingsModel.AddressSettingsModel model, AddressSettings destination)
        {
            return model.MapTo(destination);
        }



        //general (captcha) settings
        public static GeneralCommonSettingsModel.CaptchaSettingsModel ToModel(this CaptchaSettings entity)
        {
            return entity.MapTo<CaptchaSettings, GeneralCommonSettingsModel.CaptchaSettingsModel>();
        }
        public static CaptchaSettings ToEntity(this GeneralCommonSettingsModel.CaptchaSettingsModel model, CaptchaSettings destination)
        {
            return model.MapTo(destination);
        }



        //product editor settings
        public static ProductEditorSettingsModel ToModel(this ProductEditorSettings entity)
        {
            return entity.MapTo<ProductEditorSettings, ProductEditorSettingsModel>();
        }
        public static ProductEditorSettings ToEntity(this ProductEditorSettingsModel model, ProductEditorSettings destination)
        {
            return model.MapTo(destination);
        }
        #endregion

        #region Plugins

        public static PluginModel ToModel(this PluginDescriptor entity)
        {
            return entity.MapTo<PluginDescriptor, PluginModel>();
        }

        #endregion

        #region Stores

        public static StoreModel ToModel(this Store entity)
        {
            return entity.MapTo<Store, StoreModel>();
        }

        public static Store ToEntity(this StoreModel model)
        {
            return model.MapTo<StoreModel, Store>();
        }

        public static Store ToEntity(this StoreModel model, Store destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        

        #region Return request reason

        public static ReturnRequestReasonModel ToModel(this ReturnRequestReason entity)
        {
            return entity.MapTo<ReturnRequestReason, ReturnRequestReasonModel>();
        }

        public static ReturnRequestReason ToEntity(this ReturnRequestReasonModel model)
        {
            return model.MapTo<ReturnRequestReasonModel, ReturnRequestReason>();
        }

        public static ReturnRequestReason ToEntity(this ReturnRequestReasonModel model, ReturnRequestReason destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Return request action

        public static ReturnRequestActionModel ToModel(this ReturnRequestAction entity)
        {
            return entity.MapTo<ReturnRequestAction, ReturnRequestActionModel>();
        }

        public static ReturnRequestAction ToEntity(this ReturnRequestActionModel model)
        {
            return model.MapTo<ReturnRequestActionModel, ReturnRequestAction>();
        }

        public static ReturnRequestAction ToEntity(this ReturnRequestActionModel model, ReturnRequestAction destination)
        {
            return model.MapTo(destination);
        }

        #endregion

    }
}