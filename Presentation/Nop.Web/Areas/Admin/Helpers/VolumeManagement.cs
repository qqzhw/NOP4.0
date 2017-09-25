using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;  
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;

namespace Nop.Web.Areas.Admin.Helpers
{
    #region Volume Management Win32 API Wrapper
    public class VolumeAPI
    {
        public struct VolumeInformation
        {
            public string Identifier;
            public string Name;
            public string FileSystem;
            public uint SerialNumber;
            public uint Flags;
            public uint MaximumComponentLength;
            public string MountPath;
            public int SCSIPort;
            public int SCSIBus;
            public int SCSITargetId { get; set; }

            public override string ToString()
            {
                return string.Format("Volume: {0}\nName: {1}\nSystem: {2}\nSNr: {3}\nFlags: {4}\nMCL: {5}\nPath: {6}\nSCSI {7}.{8}.{9}", Identifier, Name, FileSystem, SerialNumber, Flags, MaximumComponentLength, MountPath, SCSIBus, SCSIPort, SCSITargetId);
            }



            public   override bool Equals(object obj)
            {
                try
                {
                    var vi = (VolumeInformation)obj;
                    if (((VolumeInformation)obj).Identifier == Identifier) return true;
                }
                catch
                {
                }

                return false;
            }

        }

        private const int MAX_PATH = 260;

        // Finding Volumes
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindFirstVolume(StringBuilder volumeName, UInt32 vnLength);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindNextVolume(IntPtr handle, StringBuilder volumeName, UInt32 vnLength);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindVolumeClose(IntPtr handle);

        //Retreiving Information
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumeInformation(string volume, StringBuilder volumeName, UInt32 vnLength, out UInt32 volumeSerialNumber, out UInt32 maximumComponentLength, out UInt32 fileSystemFlags, StringBuilder fileSystemName, UInt32 fsnLength);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetVolumePathNamesForVolumeName(string volumeName, StringBuilder volumePathNames, UInt32 vpnLength, out UInt32 returnLength);


        public static IEnumerable<string> GetVolumeNames()
        {
            var sbOut = new StringBuilder(MAX_PATH);
            var pHandle = FindFirstVolume(sbOut, MAX_PATH);

            var retList = new List<string>();
            do
            {
                if (!retList.Contains(sbOut.ToString()))
                    retList.Add(sbOut.ToString());
            }
            while (FindNextVolume(pHandle, sbOut, MAX_PATH));

            FindVolumeClose(pHandle);
            return retList;

        }

        public static List<VolumeInformation> GetVolumeInformation()
        {
            var retList = new List<VolumeInformation>();
            foreach (var volume in GetVolumeNames())
            {
                retList.Add(GetVolumeInformation(volume));
            }
            return retList;
        }

        private static ManagementBaseObject GetDiskDriveFromDriveLetter(string driveLetter)
        {
            ManagementObjectCollection disks = new ManagementObjectSearcher("select * from Win32_LogicalDisk where Name='" + driveLetter + "'").Get();

            foreach (ManagementObject disk in disks)
            {
                foreach (ManagementObject partition in disk.GetRelated("Win32_DiskPartition"))
                {
                    foreach (ManagementBaseObject diskDrive in partition.GetRelated("Win32_DiskDrive"))
                    {
                        return diskDrive;
                    }
                }
            }
            throw new ArgumentException("cannot trace disk, partition or drive for " + driveLetter);
        }

        public static VolumeInformation GetVolumeInformation(string volume)
        {
            var sbName = new StringBuilder(MAX_PATH);
            var sbSystem = new StringBuilder(MAX_PATH);

            var ret = new VolumeInformation
            {
                Identifier = volume,
                MountPath = GetVolumePath(volume)
            };

            GetVolumeInformation(volume, sbName, MAX_PATH, out ret.SerialNumber, out ret.MaximumComponentLength, out ret.Flags, sbSystem, MAX_PATH);
            // throw new IOException("Failed retreiving Volume Information for "+volume);



            ret.Name = sbName.ToString();
            ret.FileSystem = sbSystem.ToString();

            //get additional WMI info
            if (ret.MountPath.Length >= 2)
                try
                {
                    var disk = GetDiskDriveFromDriveLetter(ret.MountPath.Substring(0, 2));
                    ret.SCSIPort = (UInt16)disk["SCSIPort"];
                    ret.SCSIBus = (int)(UInt32)disk["SCSIBus"];
                    ret.SCSITargetId = (UInt16)disk["SCSITargetId"];
                }
                catch (Exception)
                {
                    ret.SCSIPort = -1;
                    ret.SCSIBus = -1;
                    ret.SCSITargetId = -1;
                }

            else
                ret.SCSIPort = -1;

            return ret;

        }

        public static string GetVolumePath(string volume)
        {
            uint retLen;
            var sbOut = new StringBuilder(MAX_PATH);

            if (!GetVolumePathNamesForVolumeName(volume, sbOut, MAX_PATH, out retLen))
                return "[NONE]";//throw new IOException("Failed retreiving Volume Paths");

            return sbOut.ToString();
        }
    }
    #endregion
    
}
