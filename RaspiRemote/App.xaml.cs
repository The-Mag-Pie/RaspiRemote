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

		MainPage = new StartPage();
	}

#if WINDOWS
    // Set window to be non-resizable on Windows
    private void ConfigureWindowParametersOnWindows()
    {
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (h, v) =>
        {
            var nativeWindow = h.PlatformView;
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            var presenter = appWindow.Presenter as Microsoft.UI.Windowing.OverlappedPresenter;
            presenter.IsResizable = false;
            presenter.IsMaximizable = false; // ignored (maui bug)
        });
    }

    // Set window size on Windows system to be similar to mobile screen size
    // Set window title
    protected override Window CreateWindow(IActivationState activationState)
    {
        var windowWidth = 600; // cannot be smaller because then the popups are not centered
        var windowHeight = 900;

        var window = base.CreateWindow(activationState);

        window.Title = AppInfo.Current.Name;
        window.Width = windowWidth;
        window.Height = windowHeight;

        return window;
    }
#endif
}
