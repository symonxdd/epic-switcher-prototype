using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;
using RegistryUtils;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EpicGamesAccountSwitcher
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        private OverlappedPresenter _presenter;

        private const string authDataFolder = @"C:\Users\Symon\Desktop\AuthData\";
        private const string configFolder = @"C:\Users\Symon\AppData\Local\EpicGamesLauncher\Saved\Config\Windows\";
        private const string regKeyPath = @"HKEY_CURRENT_USER\Software\Epic Games\Unreal Engine\Identifiers";
        private const string regKeyName = "AccountId";
        private const string epicGamesExecPath = @"C:\Program Files (x86)\Epic Games\Launcher\Portal\Binaries\Win32\EpicGamesLauncher.exe";

        // Source: https://www.codeproject.com/Articles/4502/RegistryMonitor-a-NET-wrapper-class-for-RegNotifyC
        private readonly RegistryMonitor _registryMonitor;

        //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        public MainWindow()
        {
            InitializeComponent();
            SetupBasicFeatures();

            PopulateAccountListBox();
            accountIdTextBlock.Text = GetCurrentAccountId();

            _registryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, "Software\\Epic Games\\Unreal Engine\\Identifiers");
            _registryMonitor.RegChanged += new EventHandler(OnRegistryChanged);
        }

        private string GetCurrentAccountId()
        {
            return (string)Registry.GetValue(regKeyPath, regKeyName, null);
        }

        private string GetSelectedAccountId()
        {
            return (string)accountListBox.SelectedValue;
        }

        private void PopulateAccountListBox()
        {
            accountListBox.Items.Clear();
            string[] files = Directory.GetFiles(authDataFolder);

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                accountListBox.Items.Add(fileName);
            }
            accountListBox.SelectedIndex = 0;
        }

        private async Task<ContentDialogResult> ShowContentDialog(string title, string content)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = mainWindow.Content.XamlRoot,
                Title = title,
                Content = content,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
            };
            return await dialog.ShowAsync();
        }

        private async void ChangeAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ShowContentDialog("Switch?", "Are you sure you want to switch accounts?");
            if (result == ContentDialogResult.Primary)
            {
                CloseEpicGamesLauncher();
                ChangeAccount();
                StartEpicGamesLauncher();
            }
            else { return; }
        }

        private void CloseEpicGamesLauncher()
        {
            string processName = "EpicGamesLauncher";

            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length != 0)
            {
                Process process = processes[0];
                process.Kill();
                process.WaitForExit();
            }
        }

        private void StartEpicGamesLauncher()
        {
            Process.Start(epicGamesExecPath);
        }


        private void ChangeAccount()
        {
            SwapFiles();
            ChangeRegKey();
            UpdateUi();
        }

        private void UpdateUi()
        {
            PopulateAccountListBox();
            accountIdTextBlock.Text = GetCurrentAccountId();
        }

        private void SwapFiles()
        {
            string configFileName = "GameUserSettings.ini";

            string fileToMove = $"{configFolder}{configFileName}";
            string newFileName = $"{GetCurrentAccountId()}.ini";
            string newFilePath = Path.Combine(authDataFolder, newFileName);
            File.Move(fileToMove, newFilePath);

            fileToMove = $"{authDataFolder}{GetSelectedAccountId()}.ini";
            newFileName = configFileName;
            newFilePath = Path.Combine(configFolder, newFileName);
            File.Move(fileToMove, newFilePath);
        }

        private void ChangeRegKey()
        {
            Registry.SetValue(regKeyPath, "AccountId", GetSelectedAccountId());
        }

        private async void NewAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var result = await ShowContentDialog("Add?", "Are you sure you want to log into another account?");
            if (result == ContentDialogResult.Primary)
            {
                CloseEpicGamesLauncher();

                string configFileName = "GameUserSettings.ini";
                string fileToMove = $"{configFolder}{configFileName}";
                string newFileName = $"{GetCurrentAccountId()}.ini";
                string newFilePath = Path.Combine(authDataFolder, newFileName);
                File.Move(fileToMove, newFilePath);

                StartEpicGamesLauncher();
                _registryMonitor.Start();
            }
            else { return; }
        }

        private void OnRegistryChanged(object sender, EventArgs e)
        {
            _registryMonitor.Stop();
            //Dispatcher.Invoke(() =>
            //{
            //    UpdateUi();
            //});
        }

        private void SetupBasicFeatures()
        {
            SetWindowSize();
            DisableWindowResize();
            EnableModernTitlebar();
        }

        private void EnableModernTitlebar()
        {
            Window window = mainWindow;
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(appTitleBar);
        }

        private void SetWindowSize()
        {
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 750, Height = 450 });
        }

        private void DisableWindowResize()
        {
            // Source: https://github.com/microsoft/WindowsAppSDK/discussions/1694

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(myWndId);
            _presenter = _appWindow.Presenter as OverlappedPresenter;

            _presenter.IsResizable = false;
        }
    }
}

