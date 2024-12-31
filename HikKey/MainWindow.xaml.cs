using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows; 
using System.Windows.Forms; 
using WindowsInput;
using Clipboard = System.Windows.Clipboard; 

namespace HikKey
{
    public partial class MainWindow : Window
    {
        private int _userId = -1;
        private bool _isDeviceConnected = false;
        private readonly InputSimulator _inputSimulator = new InputSimulator();
        private CancellationTokenSource _emulationTokenSource; 
        private NotifyIcon _trayIcon; 

        public MainWindow()
        {
            InitializeComponent();
            InitializeProgram();
            InitializeTrayIcon(); 
            this.ResizeMode = ResizeMode.CanMinimize;
            this.StateChanged += MainWindow_StateChanged;
        }
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                // Сворачиваем в трей
                _trayIcon.Visible = true;
                this.Hide(); // Скрываем окно
            }
        }

        private void InitializeProgram()
        {
            OutputTextBox.Text += 
                " ██╗    ██╗███████╗██╗     ██╗███████╗ █████╗ ██████╗ ██╗   ██╗ \n" +
                " ██║    ██║██╔════╝██║     ██║╚══███╔╝██╔══██╗██╔══██╗╚██╗ ██╔╝ \n" +
                "██║ █╗ ██║█████╗  ██║     ██║  ███╔╝ ███████║██████╔╝ ╚████╔╝ \n" +
                "██║███╗██║██╔══╝  ██║     ██║ ███╔╝  ██╔══██║██╔══██╗  ╚██╔╝  \n" +
                "╚███╔███╔╝███████╗███████╗██║███████╗██║  ██║██║  ██║   ██║   \n" + 
                "╚══╝╚══╝╚══════╝╚══════╝╚═╝╚══════╝╚═╝  ╚═╝╚═╝  ╚═╝   ╚═╝ \n";
            OutputTextBox.ScrollToEnd();
            
            
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Clear();
        }

        private void InitializeTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("icon.ico"), 
                Visible = false,
            };

            // Создаем контекстное меню
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem startMenuItem = new ToolStripMenuItem("Start KeyEmulator", null, StartEmulatorFromTray) { Name = "startMenuItem", Enabled = true };
            ToolStripMenuItem stopMenuItem = new ToolStripMenuItem("Stop KeyEmulator", null, StopEmulatorFromTray) { Name = "stopMenuItem", Enabled = false };

            contextMenu.Items.Add(startMenuItem);
            contextMenu.Items.Add(stopMenuItem);

            _trayIcon.ContextMenuStrip = contextMenu;
            _trayIcon.DoubleClick += (s, e) => ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            _trayIcon.Visible = false;
        }

        private void StartEmulatorFromTray(object sender, EventArgs e) => StartKeyboardEmulation();
        private void StopEmulatorFromTray(object sender, EventArgs e) => StopKeyboardEmulation();

        private bool InitializeSdk()
        {
            if (!SdkManager.Initialize())
            {
                OutputTextBox.Text += "Ошибка инициализации SDK.\n";
                OutputTextBox.ScrollToEnd();
                return false;
            }
            OutputTextBox.Text += "SDK успешно инициализирован.\n";
            OutputTextBox.ScrollToEnd();
            return true;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isDeviceConnected)
            {
                ConnectDevice();
            }
            else
            {
                DisconnectDevice();
            }
        }

        private void ConnectDevice()
        {
            if (_isDeviceConnected)
            {
                ResourceManager.LogoutAndCleanup(_userId);
                _isDeviceConnected = false;
                OutputTextBox.Text += "Выполнен принудительный выход перед подключением.\n";
            }

            if (!InitializeSdk())
            {
                OutputTextBox.Text += "Ошибка повторной инициализации SDK.\n";
                
                return;
            }

            var device = DeviceManager.EnumerateAndFindDevice("DS-K1F100-D8E");
            if (device == null)
            {
                OutputTextBox.Text += "Устройство не найдено.\n";
                return;
            }

            _userId = (int)DeviceManager.LoginToDevice(device.Value);
            if (_userId == -1)
            {
                OutputTextBox.Text += "Ошибка подключения к устройству.\n";
                return;
            }

            string deviceName = Encoding.ASCII.GetString(device.Value.szDeviceName).TrimEnd('\0');
            string serialNumber = Encoding.ASCII.GetString(device.Value.szSerialNumber).TrimEnd('\0');

            OutputTextBox.Text += $"Успешно подключено. UserID = {_userId}\n";
            OutputTextBox.Text += $"Имя устройства: {deviceName}\n";
            OutputTextBox.Text += $"Серийный номер устройства: {serialNumber}\n";

            _isDeviceConnected = true;
            TestButton.IsEnabled = true;
            ConnectButton.Content = "Отключить";
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestDevice();
        }

        private void TestDevice()
        {
            if (!_isDeviceConnected || _userId == -1)
            {
                OutputTextBox.Text += "Устройство не подключено.\n";
                return;
            }

            var reader = new CardReader { UserId = _userId };
            var cardInfo = reader.ActivateCard(5);
            var deviceController = new DeviceController(_userId);

            if (cardInfo != null)
            {
                OutputTextBox.Text += $"Тест устройства завершен успешно.\n";
                OutputTextBox.Text += $"Тип карты: {cardInfo.CardType}\n";
                OutputTextBox.Text += $"UID: {cardInfo.SerialNumber}\n";
                OutputTextBox.Text += $"Длина серийного номера: {cardInfo.SerialNumberLength}\n";
                OutputTextBox.Text += $"Проверочный код: {cardInfo.SelectVerifyCode}\n";
                OutputTextBox.Text += $"Длина проверочного кода: {cardInfo.SelectVerifyCodeLength}\n";
                

                bool controlResult = deviceController.ControlBeepAndLed(beepType: 3, beepCount: 2, ledType: 3, ledCount: 2);
                if (controlResult)
                {
                    OutputTextBox.Text += "Бипер и светодиод исправен\n";
                }
                else
                {
                    OutputTextBox.Text += "Ошибка активации бипера и светодиода.\n";
                }
            }
            else
            {
                OutputTextBox.Text += "Не удалось активировать карту.\n";
            }
        }

        private void DisconnectDevice()
        {
            if (_isDeviceConnected)
            {
                ResourceManager.LogoutAndCleanup(_userId);
                OutputTextBox.Text += "Успешный выход из системы и очистка ресурсов.\n";
                _isDeviceConnected = false;
                _userId = -1;

                OutputTextBox.Text += "Устройство отключено.\n";
                TestButton.IsEnabled = false;
                ConnectButton.Content = "Подключиться";
            }
            else
            {
                OutputTextBox.Text += "Устройство не было подключено.\n";
            }
        }

        private async void EmulateKeyboardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_emulationTokenSource == null || _emulationTokenSource.IsCancellationRequested)
                await StartKeyboardEmulation();
            else
                StopKeyboardEmulation();
        }

private async Task StartKeyboardEmulation()
{
    _emulationTokenSource = new CancellationTokenSource();
    CancellationToken token = _emulationTokenSource.Token;

    OutputTextBox.Text += "Эмуляция клавиатуры запущена...\n";
    _trayIcon.Visible = true;
    this.Hide();

    _trayIcon.ContextMenuStrip.Items["startMenuItem"].Enabled = false;
    _trayIcon.ContextMenuStrip.Items["stopMenuItem"].Enabled = true;

    try
    {
        await Task.Run(async () =>  
        {
            while (!token.IsCancellationRequested)
            {
                var reader = new CardReader { UserId = _userId };
                var cardInfo = reader.ActivateCard(5);

                var deviceController = new DeviceController(_userId);

                if (cardInfo != null)
                {
                    string rawSerialNumber = cardInfo.SerialNumber.Replace("-", "");
                    
                    
                    Dispatcher.Invoke(() =>
                    {
                        OutputTextBox.Text += $"UID: {rawSerialNumber}\n";
                        Clipboard.SetText(rawSerialNumber);
                        //OutputTextBox.Text += "Серийный номер скопирован в буфер обмена.\n";
                        _inputSimulator.Keyboard.TextEntry(rawSerialNumber);
                        //OutputTextBox.Text += "Серийный номер вставлен в активное окно.\n";
                    });

                    
                    bool controlResult = deviceController.ControlBeepAndLed(beepType: 3, beepCount: 1, ledType: 3, ledCount: 1);
                    if (controlResult)
                    {
                        //Dispatcher.Invoke(() => OutputTextBox.Text += "Бипер и светодиод активированы после считывания карты.\n");
                    }
                    else
                    {
                        Dispatcher.Invoke(() => OutputTextBox.Text += "Ошибка активации бипера и светодиода после считывания карты.\n");
                    }
                }
               

                await Task.Delay(550, token);
            }
        }, token);
    }
    catch (TaskCanceledException)
    {
        OutputTextBox.Text += "Эмуляция клавиатуры остановлена.\n";
    }
}

        private void StopKeyboardEmulation()
        {
            _emulationTokenSource?.Cancel();
            _trayIcon.ContextMenuStrip.Items["startMenuItem"].Enabled = true;
            _trayIcon.ContextMenuStrip.Items["stopMenuItem"].Enabled = false;
        }
    }
}
