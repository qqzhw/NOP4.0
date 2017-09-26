using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{

	public class DeviceSettings : ISettings
	{
		public DeviceSettings()
		{

		}

		public bool StartEnabled { get; set; }

		public bool StopEnabled { get; set; }


		public bool CheckEnabled { get; set; }


		public bool IsConnect { get; set; }
		public bool CanConnect { get; set; }

		public bool CanClosed { get; set; }

		public bool ShowRate { get; set; }
		public int Status { get; set; }
		
	}
}
