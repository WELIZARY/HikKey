using System;
using System.Runtime.InteropServices;

namespace HikKey;

public static class SdkManager
{
    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern bool USB_SDK_Init();

    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern int USB_SDK_GetLastError();

    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern bool USB_SDK_Cleanup();

    [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
    private static extern bool USB_SDK_SetLogToFile(uint logLevel, string logDir, bool autoDel);

    public static bool Initialize()
    {
        bool initResult = USB_SDK_Init();
        if (!initResult)
        {
            int errorCode = USB_SDK_GetLastError();
            Console.WriteLine($"Ошибка инициализации SDK. Код ошибки: {errorCode}");
            return false;
        }
        Console.WriteLine("SDK успешно инициализирован.");

        bool logResult = USB_SDK_SetLogToFile(3, "C:\\Logs", true);
        if (!logResult)
        {
            int errorCode = USB_SDK_GetLastError();
            Console.WriteLine($"Ошибка настройки логирования. Код ошибки: {errorCode}");
            return false;
        }

        return true;
    }

    public static bool Cleanup()
    {
        Console.WriteLine("Попытка очистки ресурсов...");
        bool cleanupResult = USB_SDK_Cleanup();
        if (!cleanupResult)
        {
            int errorCode = USB_SDK_GetLastError();
            Console.WriteLine($"Ошибка очистки ресурсов. Код ошибки: {errorCode}");
            return false;
        }
        Console.WriteLine("Ресурсы успешно очищены.");
        return true;
    }


    public const uint UsbSdkGetActivateCard = 0x0104; // Команда активации карты

    public static int GetLastError()
    {
        return USB_SDK_GetLastError();
    }
}