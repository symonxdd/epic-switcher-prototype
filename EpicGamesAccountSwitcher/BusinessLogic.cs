using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using RegistryUtils;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EpicGamesAccountSwitcher
{
    internal static class BusinessLogic
    {
        private const string AppSettingsFolderName = "EpicGamesAccountSwitcher";
        private const string AppEpicAccountsFolderName = "AccountData";
        private static readonly RegistryMonitor _registryMonitor;
        private static Window _mainWindow;
        private static ComboBox _accountComboBox;
        private static TextBlock _accountIdTextBlock;
        private static TextBlock _noAccountsWarningTextBlock;
        private static TextBlock _accountsCountTextBlock;
        private static Button _switchAccountButton;
        private static string _appEpicAccountsFolderPath;
        private static string _epicConfigFolder;
        private static string _regKeyPath;
        private static string _regKeyFullPath;
        private static string _regKeyName;
        private static string _epicGamesExecPath;

        public static string SelectedAccountId { get; set; }
        public static string CurrentAccountId { get => (string)Registry.GetValue(_regKeyFullPath, _regKeyName, null); }

        static BusinessLogic()
        {
            string localAppDataPath;
            string regKeyHive = "HKEY_CURRENT_USER";
            //#if DEBUG
            //            localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //#else
            //            localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            //#endif
            localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _appEpicAccountsFolderPath = $"{localAppDataPath}\\{AppSettingsFolderName}\\{AppEpicAccountsFolderName}\\";
            _epicConfigFolder = $"{localAppDataPath}\\EpicGamesLauncher\\Saved\\Config\\Windows\\";
            _epicGamesExecPath = $"C:\\Program Files (x86)\\Epic Games\\Launcher\\Portal\\Binaries\\Win32\\EpicGamesLauncher.exe";
            _regKeyPath = $"Software\\Epic Games\\Unreal Engine\\Identifiers";
            _regKeyFullPath = $"{regKeyHive}\\Software\\Epic Games\\Unreal Engine\\Identifiers";
            _regKeyName = "AccountId";

            // implement only on first run
            Directory.CreateDirectory(_appEpicAccountsFolderPath);

            _registryMonitor = new RegistryMonitor(RegistryHive.CurrentUser, _regKeyPath);
            _registryMonitor.RegChanged += new EventHandler(OnRegistryChanged);
        }

        public static void Initialize(
            Window mainWindow,
            ComboBox accountComboBox,
            TextBlock accountIdTextBlock,
            TextBlock noAccountsWarningTextBlock,
            TextBlock accountsCountTextBlock,
            Button switchAccountButton)
        {
            _mainWindow = mainWindow;
            _accountComboBox = accountComboBox;
            _accountIdTextBlock = accountIdTextBlock;
            _noAccountsWarningTextBlock = noAccountsWarningTextBlock;
            _switchAccountButton = switchAccountButton;
            _accountsCountTextBlock = accountsCountTextBlock;
        }

        public static void SwapFiles()
        {
            string configFileName = "GameUserSettings.ini";

            string fileToMove = $"{_epicConfigFolder}{configFileName}";
            string newFileName = $"{CurrentAccountId}.ini";
            string newFilePath = Path.Combine(_appEpicAccountsFolderPath, newFileName);
            File.Move(fileToMove, newFilePath);

            fileToMove = $"{_appEpicAccountsFolderPath}{SelectedAccountId}.ini";
            newFileName = configFileName;
            newFilePath = Path.Combine(_epicConfigFolder, newFileName);
            File.Move(fileToMove, newFilePath);
        }

        public static void ChangeRegKey() => Registry.SetValue(_regKeyFullPath, _regKeyName, SelectedAccountId);

        public static async Task<ContentDialogResult> ShowContentDialog(string title, string content)
        {
            ContentDialog dialog = new()
            {
                XamlRoot = _mainWindow.Content.XamlRoot,
                Title = title,
                Content = content,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
            };
            return await dialog.ShowAsync();
        }

        public static void CloseEpicGamesLauncher()
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

        public static void StartEpicGamesLauncher() => Process.Start(_epicGamesExecPath);

        public static void PopulateAccountListBox()
        {
            string[] files = Directory.GetFiles(_appEpicAccountsFolderPath);
            int accountsCount = 0;

            _accountComboBox.Items.Clear();
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                _accountComboBox.Items.Add(fileName);
                accountsCount++;
            }
            _accountsCountTextBlock.Text = $"({accountsCount})";
            _accountComboBox.SelectedIndex = 0;
        }

        internal static void SwitchAccount()
        {
            CloseEpicGamesLauncher();
            SwapFiles();
            ChangeRegKey(); // actually optional
            UpdateUI();
            StartEpicGamesLauncher();
        }

        internal static void AddAccount()
        {
            CloseEpicGamesLauncher();

            string configFileName = "GameUserSettings.ini";
            string fileToMove = $"{_epicConfigFolder}{configFileName}";
            string newFileName = $"{CurrentAccountId}.ini";
            string newFilePath = Path.Combine(_appEpicAccountsFolderPath, newFileName);
            File.Move(fileToMove, newFilePath);

            StartEpicGamesLauncher();
            _registryMonitor.Start();
        }

        internal static void UpdateUI()
        {
            _accountIdTextBlock.Text = CurrentAccountId;
            PopulateAccountListBox();

            if (_accountComboBox.Items.Count == 0)
            {
                _switchAccountButton.IsEnabled = false;
                _accountComboBox.Visibility = Visibility.Collapsed;
                _noAccountsWarningTextBlock.Visibility = Visibility.Visible;
            }
        }

        private static void OnRegistryChanged(object sender, EventArgs e)
        {
            _registryMonitor.Stop();
            //Dispatcher.Invoke(() =>
            //{
            //    UpdateUI();
            //});
        }
    }
}
