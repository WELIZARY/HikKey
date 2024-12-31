using System;
using System.Runtime.InteropServices;

namespace HikKey;

public class CardReader
{
    [DllImport("HCUsbSDK.dll", CharSet = CharSet.Ansi, SetLastError = true, CallingConvention = CallingConvention.StdCall)]
    public static extern bool USB_SDK_GetDeviceConfig(
        int lUserId,
        int dwCommand,
        ref UsbConfigInputInfo lpInput,
        ref UsbConfigOutputInfo lpOutput);

    public int UserId { get; set; }

    public CardInfo ActivateCard(int waitSeconds)
    {
        if (waitSeconds < 0 || waitSeconds > 255)
        {
            Console.WriteLine("Диапазон времени ожидания 0-255 секунд.");
            return null;
        }

        Console.WriteLine("Считывание карты...");

        UsbSdkWaitSecond waitSecond = new UsbSdkWaitSecond
        {
            dwSize = (uint)Marshal.SizeOf(typeof(UsbSdkWaitSecond)),
            byWait = (byte)waitSeconds,
            byRes = new byte[27]
        };

        IntPtr waitSecondPtr = Marshal.AllocHGlobal(Marshal.SizeOf(waitSecond));
        Marshal.StructureToPtr(waitSecond, waitSecondPtr, false);

        UsbSdkActivateCardRes activateCardRes = new UsbSdkActivateCardRes
        {
            dwSize = (uint)Marshal.SizeOf(typeof(UsbSdkActivateCardRes)),
            bySerial = new byte[10],
            bySelectVerify = new byte[3],
            byRes = new byte[12]
        };

        IntPtr activateCardResPtr = Marshal.AllocHGlobal(Marshal.SizeOf(activateCardRes));
        Marshal.StructureToPtr(activateCardRes, activateCardResPtr, false);

        UsbConfigInputInfo inputInfo = new UsbConfigInputInfo
        {
            lpInBuffer = waitSecondPtr,
            dwInBufferSize = (uint)Marshal.SizeOf(waitSecond),
            byRes = new byte[48]
        };

        UsbConfigOutputInfo outputInfo = new UsbConfigOutputInfo
        {
            lpOutBuffer = activateCardResPtr,
            dwOutBufferSize = (uint)Marshal.SizeOf(activateCardRes),
            byRes = new byte[56]
        };

        try
        {
            Console.WriteLine("Передача команды на активацию карты...");

            bool result = USB_SDK_GetDeviceConfig(UserId, (int)SdkManager.UsbSdkGetActivateCard, ref inputInfo, ref outputInfo);

            activateCardRes = Marshal.PtrToStructure<UsbSdkActivateCardRes>(outputInfo.lpOutBuffer);

            if (!result)
            {
                Console.WriteLine("Ошибка считывания карты.");
                return null;
            }

            CardInfo cardInfo = new CardInfo
            {
                CardType = activateCardRes.byCardType.ToString(),
                SerialNumber = BitConverter.ToString(activateCardRes.bySerial, 0, activateCardRes.bySerialLen),
                SerialNumberLength = activateCardRes.bySerialLen,
                SelectVerifyCode = BitConverter.ToString(activateCardRes.bySelectVerify, 0, activateCardRes.bySelectVerifyLen),
                SelectVerifyCodeLength = activateCardRes.bySelectVerifyLen
            };




            return cardInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return null;
        }
        finally
        {
            Console.WriteLine("Освобождение памяти...");
            Marshal.FreeHGlobal(waitSecondPtr);
            Marshal.FreeHGlobal(activateCardResPtr);
            Console.WriteLine("Карта успешно считана.");
        }
    }
}
