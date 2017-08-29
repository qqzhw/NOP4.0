using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Nop.Web.Models.Directory
{
     
	[DataContract]
	public class ChunkMetaData
	{
		[DataMember(Name = "uploadUid")]
		public string UploadUid { get; set; }
		[DataMember(Name = "fileName")]
		public string FileName { get; set; }
		[DataMember(Name = "contentType")]
		public string ContentType { get; set; }
		[DataMember(Name = "chunkIndex")]
		public long ChunkIndex { get; set; }
		[DataMember(Name = "totalChunks")]
		public long TotalChunks { get; set; }
		[DataMember(Name = "totalFileSize")]
		public long TotalFileSize { get; set; }
	}

	public class FilesResult
	{
		public bool uploaded { get; set; }
		public string fileUid { get; set; }
	}
}
