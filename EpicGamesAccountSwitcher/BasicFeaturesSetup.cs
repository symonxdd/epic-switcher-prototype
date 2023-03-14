using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI;

namespace EpicGamesAccountSwitcher
{
    internal static class BasicFeaturesSetup
    {
        private static Window _mainWindow;
        private static FrameworkElement _appTitleBar;
        private static AppWindow _appWindow;
        private static OverlappedPresenter _presenter;

        public static void Setup(Window mainWindow, FrameworkElement appTitleBar)
        {
            _mainWindow = mainWindow;
            _appTitleBar = appTitleBar;

            InitAppWindowAndPresenter();
            SetAppIcon();
            DisableWindowResize();
            EnableModernTitlebar();
            SetWindowSize();
        }

        private static void SetAppIcon()
        {
            _appWindow.SetIcon(@"Assets\app_logo.ico");
        }
       
        private static void EnableModernTitlebar()
        {
            Window window = _mainWindow;
            window.ExtendsContentIntoTitleBar = true;
            window.SetTitleBar(_appTitleBar);
        }

        private static void SetWindowSize()
        {
            _appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 750, Height = 450 });
        }

        private static void DisableWindowResize()
        {            
            _presenter.IsResizable = false;
        }

        private static void InitAppWindowAndPresenter()
        {
            // Source: https://github.com/microsoft/WindowsAppSDK/discussions/1694

            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(_mainWindow);
            WindowId myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
            _appWindow = AppWindow.GetFromWindowId(myWndId);
            _presenter = _appWindow.Presenter as OverlappedPresenter;
        }
    }
}
