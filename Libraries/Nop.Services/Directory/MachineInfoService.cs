using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Directory
{
    public class MachineInfoService : IMachineInfoService
    { 
        public MachineInfoService()
        {
            
        }
        public void GetCPUSerialNumber()
        {
            using (var management = new ManagementObjectSearcher())
            {
                ObjectQuery query = new ObjectQuery($"select * from {WMIType.Win32_Processor.ToString()} ");
                management.Query = query;
                management.Scope=new ManagementScope(@"root\CIMV2");
                //ManagementObjectCollection queryCollection = management.Get();
                //foreach (var item in queryCollection)
                //{
                //    var Manufacturer=item.GetPropertyValue("Manufacturer");
                //    var Manufacturer1 = item.GetPropertyValue("Name");
                //    var Manufacturer2 = item.GetPropertyValue("Description");
                //    var Manufacture3r = item.GetPropertyValue("ProcessorID");                 
                //    var Manufacturer5 = item.GetPropertyValue("AddressWidth");
                //    var Manufacturer6 = item.GetPropertyValue("NumberOfCores"); 
                //}
                //ObjectQuery query2 = new ObjectQuery($"select * from {WMIType.MSAcpi_ThermalZoneTemperature.ToString()} ");
                //management.Query = query2;
                //management.Scope = new ManagementScope(@"root\CIMV2");
                //ManagementObjectCollection queryCollection = management.Get();
            }
           
        }
    }
}
