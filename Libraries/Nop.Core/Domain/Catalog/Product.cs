using System;
using System.Collections.Generic;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Seo;
using Nop.Core.Domain.Stores;

namespace Nop.Core.Domain.Catalog
{
	/// <summary>
	/// Represents a product
	/// </summary>
	public partial class Product : BaseEntity, ISlugSupported, IAclSupported
	{

		/// <summary>
		/// Gets or sets the product type identifier
		/// </summary>
		public int ProductTypeId { get; set; }

		/// <summary>
		/// Gets or sets the name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the short description
		/// </summary>
		public string ShortDescription { get; set; }
		/// <summary>
		/// Gets or sets the full description
		/// </summary>
		public string Remark { get; set; }

		public string DeviceId { get; set; }
		public string VendorId { get; set; }
        public string DefaultDir { get; set; }

        public bool IsOpen { get; set; }

		public bool SubjectToAcl { get; set; }

		public int RegAddress { get; set; }

		public bool DisableButton { get; set; }

		public int DisplayOrder { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the entity is published
		/// </summary>
		public bool Published { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether the entity has been deleted
		/// </summary>
		public bool Deleted { get; set; }

		/// <summary>
		/// Gets or sets the date and time of product creation
		/// </summary>
		public DateTime CreatedOn { get; set; }

		public DateTime UpdatedWriteOn { get; set; }

	}
}