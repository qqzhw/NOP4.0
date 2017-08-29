using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Web.Models.Directory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Controllers
{
	public class UploadController : BaseAdminController
	{
		private readonly IHostingEnvironment _environment;
		public UploadController(IHostingEnvironment  environment)
		{
			_environment = environment;
		}
		[HttpPost]
		public ActionResult Submit(ICollection<IFormFile> files)
		{
			if (files != null)
			{
				TempData["UploadedFiles"] = GetFileInfo(files);
			}

			return RedirectToRoute("Demo", new { section = "upload", example = "result" });
		}

		public async Task<ActionResult> Save(ICollection<IFormFile> files)
		{
			// The Name of the Upload component is "files"
			if (files != null)
			{
				foreach (var file in files)
				{
					// Some browsers send file names with full path. This needs to be stripped.
					 var fileName = Path.GetFileName(file.FileName);
					 var physicalPath = Path.Combine(CommonHelper.UploadFilePath, fileName);

					// The files are not actually saved in this demo
					//  file.CopyToAsync(physicalPath);
					if (file.Length > 0)
					{
						using (var fileStream = new FileStream(Path.Combine(CommonHelper.UploadFilePath, file.FileName), FileMode.Create))
						{
							await file.CopyToAsync(fileStream);
						}
					}
				}
			}

			// Return an empty string to signify success
			return Content("");
		}

		public ActionResult Remove(string[] fileNames)
		{
			// The parameter of the Remove action must be called "fileNames"

			if (fileNames != null)
			{
				foreach (var fullName in fileNames)
				{
					var fileName = Path.GetFileName(fullName);
					var physicalPath = Path.Combine(CommonHelper.UploadFilePath, fileName);

					// TODO: Verify user permissions

					if (System.IO.File.Exists(physicalPath))
					{
						// The files are not actually removed in this demo
						  System.IO.File.Delete(physicalPath);
					}
				}
			}

			// Return an empty string to signify success
			return Content("");
		}

		public void AppendToFile(string fullPath, Stream content)
		{
			try
			{
				using (FileStream stream = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
				{
					using (content)
					{
						content.CopyTo(stream);
					}
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
		}

		public async Task<ActionResult> ChunkSave(ICollection<IFormFile> files, string metaData)
		{
			if (metaData == null)
			{
				return await Save(files);
			}

			MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(metaData));
			var serializer = new DataContractJsonSerializer(typeof(ChunkMetaData));
			ChunkMetaData somemetaData = serializer.ReadObject(ms) as ChunkMetaData;
			string path = String.Empty;
			// The Name of the Upload component is "files"
			if (files != null)
			{
				foreach (var file in files)
				{
					 path = Path.Combine(CommonHelper.UploadFilePath, somemetaData.FileName);

					 AppendToFile(path, file.OpenReadStream());
				}
			}

			FilesResult fileBlob = new FilesResult();
			fileBlob.uploaded = somemetaData.TotalChunks - 1 <= somemetaData.ChunkIndex;
			fileBlob.fileUid = somemetaData.UploadUid;

			return Json(fileBlob);
		}

		private IEnumerable<string> GetFileInfo(ICollection<IFormFile> files)
		{
			return
				from a in files
				where a != null
				select string.Format("{0} ({1} bytes)", Path.GetFileName(a.FileName), a.Length);
		}
	}
}
