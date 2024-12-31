//DeviceManager.cs

using System.Runtime.InteropServices;
using System.Text;

namespace HikKey;

public delegate void EnumDeviceCallBack(ref UsbSdkDeviceInfo deviceInfo, IntPtr pUser);
public static class DeviceManager
{
    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern bool USB_SDK_EnumDevice(EnumDeviceCallBack cbEnumDeviceCallBack, IntPtr pUser);

    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern long USB_SDK_Login(ref UsbSdkUserLoginInfo loginInfo, ref UsbSdkDeviceRegRes deviceInfo);

    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern bool USB_SDK_Logout(int lUserId);

    private static UsbSdkDeviceInfo? _targetDevice = null;

    public static UsbSdkDeviceInfo? EnumerateAndFindDevice(string deviceName)
    {
        Console.WriteLine("Поиск устройства...");
        bool enumResult = USB_SDK_EnumDevice(DeviceEnumCallback, IntPtr.Zero);
        if (!enumResult)
        {
            int errorCode = SdkManager.GetLastError();
            Console.WriteLine($"Ошибка перечисления устройств. Код ошибки: {errorCode}");
            return null;
        }
        return _targetDevice;
    }

    private static void DeviceEnumCallback(ref UsbSdkDeviceInfo deviceInfo, IntPtr pUser)
    {
        string foundDeviceName = Encoding.ASCII.GetString(deviceInfo.szDeviceName).TrimEnd('\0');
        string serialNumber = Encoding.ASCII.GetString(deviceInfo.szSerialNumber).TrimEnd('\0');
        
        Console.WriteLine($"Устройство найдено: {foundDeviceName}, Серийный номер: {serialNumber}");

        if (foundDeviceName == "DS-K1F100-D8E")
        {
            _targetDevice = deviceInfo;
        }
    }
                //Тест метода активации бипера
            
            
    public static long LoginToDevice(UsbSdkDeviceInfo device)
    {
        
        Console.WriteLine("Подключение к устройству...");
        
        UsbSdkUserLoginInfo loginInfo = new UsbSdkUserLoginInfo
        {
            dwSize = (uint)Marshal.SizeOf(typeof(UsbSdkUserLoginInfo)),
            dwTimeout = 5000,
            dwVID = device.dwVID,
            dwPID = device.dwPID,
            szUserName = Encoding.ASCII.GetBytes("admin".PadRight(32, '\0')),
            szPassword = Encoding.ASCII.GetBytes("12345".PadRight(16, '\0')),
            szSerialNumber = device.szSerialNumber, 
            byRes = new byte[80]
        };

        UsbSdkDeviceRegRes deviceInfo = new UsbSdkDeviceRegRes
        {
            dwSize = (uint)Marshal.SizeOf(typeof(UsbSdkDeviceRegRes)),
            byRes = new byte[40]
        };
        
        long userId = USB_SDK_Login(ref loginInfo, ref deviceInfo);
        if (userId == -1)
        {
            int errorCode = SdkManager.GetLastError();
            Console.WriteLine($"Ошибка подключения к устройству. Код ошибки: {errorCode}");
            return -1;
        }

        string deviceName = Encoding.ASCII.GetString(deviceInfo.szDeviceName).TrimEnd('\0');
        string serialNumber = Encoding.ASCII.GetString(deviceInfo.szSerialNumber).TrimEnd('\0');
        Console.WriteLine($"Подключено:");
        Console.WriteLine($"Имя: {deviceName}");
        Console.WriteLine($"Серийный номер устройства: {serialNumber}");
        Console.WriteLine($"Версия ПО: {deviceInfo.dwSoftwareVersion:X}");

        return userId;
    }

    // Новый метод для выполнения только выхода
    public static bool Logout(long userId)
    {
        return USB_SDK_Logout((int)userId);
    }

    // Оставляем метод для выполнения выхода и очистки
    public static void LogoutAndCleanup(long userId)
    {
        Console.WriteLine("Попытка выхода из системы...");
        bool logoutResult = USB_SDK_Logout((int)userId);
        if (!logoutResult)
        {
            int errorCode = SdkManager.GetLastError();
            Console.WriteLine($"Ошибка выхода из системы. Код ошибки: {errorCode}");
        }
        else
        {
            Console.WriteLine("Успешный выход из системы.");
        }

        SdkManager.Cleanup();
    }
}