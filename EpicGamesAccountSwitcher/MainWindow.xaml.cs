using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using System;

namespace EpicGamesAccountSwitcher
{
    public sealed partial class MainWindow : Window
    {
        private AppWindow _appWindow;
        private OverlappedPresenter _presenter;

        public MainWindow()
        {
            InitializeComponent();
            SetupBasicFeatures();
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

