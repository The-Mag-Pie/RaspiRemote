using RaspiRemote.Pages;

namespace RaspiRemote;

public partial class App : Application
{
	public App(SshClientContainer sshClientContainer)
	{
		InitializeComponent();

#if WINDOWS
        ConfigureWindowParametersOnWindows();
#endif

        // Force light theme (disable dark theme)
        Current.UserAppTheme = AppTheme.Light;

		MainPage = new NavigationPage(new SelectDevicePage());
	}

#if WINDOWS
    // Set window to be non-resizable on Windows
    private void ConfigureWindowParametersOnWindows()
    {
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (h, v) =>
        {
            var nativeWindow = h.PlatformView;
            //nativeWindow.ExtendsContentIntoTitleBar = false; // workaround for IsMaximizable bug
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsMaximizable = false; // ignored (maui bug)
        });
    }

    // Set window size on Windows system to be similar to mobile screen size
    // Set window location to the center of the screen - disabled
    // Set window title
    protected override Window CreateWindow(IActivationState activationState)
    {
        var windowWidth = 600; // cannot be smaller because then the popups are not centered
        var windowHeight = 900;

        var window = base.CreateWindow(activationState);

        window.Title = AppInfo.Current.Name;
        window.Width = windowWidth;
        window.Height = windowHeight;

        //window.Activated += async (s, e) =>
        //{
        //    window.Width = windowWidth;
        //    window.Height = windowHeight;

        //    await window.Dispatcher.DispatchAsync(() => { });

        //    var display = DeviceDisplay.Current.MainDisplayInfo;

        //    window.X = (display.Width / display.Density - window.Width) / 2;
        //    window.Y = (display.Height / display.Density - window.Height) / 2;
        //};

        return window;
    }
#endif
}
