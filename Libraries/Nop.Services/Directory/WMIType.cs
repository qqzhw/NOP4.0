using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Directory
{
    public enum WMIType
    {
        // 硬件 
        Win32_Processor, // CPU 处理器 
        Win32_PhysicalMemory, // 物理内存条 
        Win32_Keyboard, // 键盘 
        Win32_PointingDevice, // 点输入设备，包括鼠标。 
        Win32_FloppyDrive, // 软盘驱动器 
        Win32_DiskDrive, // 硬盘驱动器 
        Win32_CDROMDrive, // 光盘驱动器 
        Win32_BaseBoard, // 主板 
        Win32_BIOS, // BIOS 芯片 
        Win32_ParallelPort, // 并口 
        Win32_SerialPort, // 串口 
        Win32_SerialPortConfiguration, // 串口配置 
        Win32_SoundDevice, // 多媒体设置，一般指声卡。 
        Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP) 
        Win32_USBController, // USB 控制器        
        Win32_USBControllerDevice,
        Win32_NetworkAdapter, // 网络适配器 
        Win32_NetworkAdapterConfiguration, // 网络适配器设置 
        Win32_NetworkAdapterSetting,//关联网络适配器及其配置设置
        Win32_Printer, // 打印机 
        Win32_PrinterConfiguration, // 打印机设置 
        Win32_PrintJob, // 打印机任务 
        Win32_TCPIPPrinterPort, // 打印机端口 
        Win32_POTSModem, // MODEM 
        Win32_POTSModemToSerialPort, // MODEM 端口 
        Win32_DesktopMonitor, // 显示器 
        Win32_DisplayConfiguration, // 显卡 
        Win32_DisplayControllerConfiguration, // 显卡设置 
        Win32_VideoController, // 显卡细节。 
        Win32_VideoSettings, // 显卡支持的显示模式。
        Win32_PortConnector,
        Win32_DMAChannel,
        Win32_IDEController,
        Win32_IDEControllerDevice,
        Win32_TemperatureProbe,//  表示温度传感器（电子温度计）的特性。
        Win32_PNPEntity,//即插即用设备属性

        // 操作系统 
        Win32_Account ,//--帐户
        Win32_Directory,//目录
        Win32_SubDirectory,//子目录
        Win32_TimeZone, // 时区 
        Win32_SystemDriver, // 驱动程序 
        Win32_DiskPartition, // 磁盘分区 
        Win32_LogicalDisk, // 逻辑磁盘 
        Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。 
        Win32_LogicalMemoryConfiguration, // 逻辑内存配置 
        Win32_LogicalDiskRootDirectory,
        Win32_PageFile, // 系统页文件信息 
        Win32_PageFileSetting, // 页文件设置 
        Win32_BootConfiguration, // 系统启动配置 
        Win32_ComputerSystem, // 计算机信息简要 
        Win32_OperatingSystem, // 操作系统信息 
        Win32_StartupCommand, // 系统自动启动程序 
        Win32_Service, // 系统安装的服务 
        Win32_Group, // 系统管理组 
        Win32_GroupUser, // 系统组帐号 
        Win32_UserAccount, // 用户帐号 
        Win32_Process, // 系统进程 
        Win32_ProcessStartup,//启动
        Win32_Thread, // 系统线程 
        Win32_Share, // 共享 
        Win32_NetworkClient, // 已安装的网络客户端 
        Win32_NetworkProtocol, // 已安装的网络协议
         //存储
        Win32_Volume,
        Win32_ShadowContext,
        Win32_NTLogEventLog, //事件日志
        MSAcpi_ThermalZoneTemperature, //CPU温度
    }
}
