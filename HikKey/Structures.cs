//Structures.cs

using System.Runtime.InteropServices;

namespace HikKey;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbSdkDeviceInfo
{
    public uint dwSize;
    public uint dwVID;
    public uint dwPID;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] szManufacturer;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] szDeviceName;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
    public byte[] szSerialNumber;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
    public byte[] byRes;

    public void Init()
    {
        szManufacturer = new byte[32];
        szDeviceName = new byte[32];
        szSerialNumber = new byte[48];
        byRes = new byte[68];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbSdkUserLoginInfo
{
    public uint dwSize;
    public uint dwTimeout;
    public uint dwVID;
    public uint dwPID;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] szUserName;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] szPassword;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
    public byte[] szSerialNumber;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
    public byte[] byRes;

    public void Init()
    {
        szUserName = new byte[32];
        szPassword = new byte[16];
        szSerialNumber = new byte[48];
        byRes = new byte[80];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbSdkDeviceRegRes
{
    public uint dwSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] szDeviceName;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
    public byte[] szSerialNumber;
    public uint dwSoftwareVersion;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] byRes;

    public void Init()
    {
        szDeviceName = new byte[32];
        szSerialNumber = new byte[48];
        byRes = new byte[40];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbSdkWaitSecond
{
    public uint dwSize;
    public byte byWait;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 27)]
    public byte[] byRes;

    public void Init()
    {
        byRes = new byte[27];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbSdkActivateCardRes
{
    public uint dwSize;
    public byte byCardType;
    public byte bySerialLen;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] bySerial;
    public byte bySelectVerifyLen;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] bySelectVerify;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public byte[] byRes;

    public void Init()
    {
        bySerial = new byte[10];
        bySelectVerify = new byte[3];
        byRes = new byte[12];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbConfigInputInfo
{
    public IntPtr lpCondBuffer;
    public uint dwCondBufferSize;
    public IntPtr lpInBuffer;
    public uint dwInBufferSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
    public byte[] byRes;

    public void Init()
    {
        byRes = new byte[48];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct UsbConfigOutputInfo
{
    public IntPtr lpOutBuffer;
    public uint dwOutBufferSize;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 56)]
    public byte[] byRes;

    public void Init()
    {
        byRes = new byte[56];
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BeepAndLedConfig
{
    public uint dwSize;
    public byte byBeepType;
    public byte byBeepCount;
    public byte byLedType;
    public byte byLedCount;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public byte[] byRes;

    public void Init()
    {
        dwSize = (uint)Marshal.SizeOf(typeof(BeepAndLedConfig));
        byRes = new byte[24];
    }


}
