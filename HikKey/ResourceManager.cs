// ResourceManager.cs

namespace HikKey;

public static class ResourceManager
{
    public static void LogoutAndCleanup(long userId)
    {
        // Попытка выхода из системы
        Console.WriteLine("Попытка выхода из системы...");
        bool logoutResult = DeviceManager.Logout(userId);
        if (!logoutResult)
        {
            int errorCode = SdkManager.GetLastError();
            Console.WriteLine($"Ошибка выхода из системы. Код ошибки: {errorCode}");
        }
        else
        {
            Console.WriteLine("Успешный выход из системы.");
        }

        // Вызов Cleanup без дополнительных сообщений
        SdkManager.Cleanup();
    }
}