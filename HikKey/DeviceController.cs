//DeviceController.cs
using System;
using System.Runtime.InteropServices;

namespace HikKey
{
    public class DeviceController
    {
        [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern bool USB_SDK_SetDeviceConfig(int lUserId, int dwCommand, ref UsbConfigInputInfo lpInput, ref UsbConfigOutputInfo lpOutput);

        [DllImport("HCUsbSDK.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int USB_SDK_GetLastError();

        private readonly int _userId;

        public DeviceController(int userId)
        {
            _userId = userId;
        }



        public bool ControlBeepAndLed(int beepType, int beepCount, int ledType, int ledCount)
        {


            BeepAndLedConfig config = new BeepAndLedConfig();
            config.Init();
            config.byBeepType = (byte)beepType;
            config.byBeepCount = (byte)beepCount;
            config.byLedType = (byte)ledType;
            config.byLedCount = (byte)ledCount;

            UsbConfigInputInfo inputInfo = new UsbConfigInputInfo();
            inputInfo.Init();
            inputInfo.lpInBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(config));
            inputInfo.dwInBufferSize = (uint)Marshal.SizeOf(config);
            Marshal.StructureToPtr(config, inputInfo.lpInBuffer, false);

            UsbConfigOutputInfo outputInfo = new UsbConfigOutputInfo();
            outputInfo.Init();

            bool result = USB_SDK_SetDeviceConfig(_userId, 0x0100, ref inputInfo, ref outputInfo);

            if (!result)
            {
                int errorCode = USB_SDK_GetLastError();
                Console.WriteLine($"Ошибка управления бипером и светодиодом. Код ошибки: {errorCode}");
            }
            else
            {
                Console.WriteLine("Управление бипером и светодиодом выполнено успешно.");
            }

            Marshal.FreeHGlobal(inputInfo.lpInBuffer);

            return result;
        }
    }
}