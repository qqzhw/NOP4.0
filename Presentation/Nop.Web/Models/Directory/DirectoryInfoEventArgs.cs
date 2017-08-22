using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Models.Directory
{

	public class DirectoryInfoEventArgs : EventArgs
	{
		public DirectoryInfo DirectoryInfo;
		public DirectoryInfoEventArgs(DirectoryInfo directoryInfo)
		{
			this.DirectoryInfo = directoryInfo;
		}
	}
}
