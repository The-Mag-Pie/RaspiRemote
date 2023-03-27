using RaspiRemote.Pages;

namespace RaspiRemote;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Force light theme (disable dark theme)
		Current.UserAppTheme = AppTheme.Light;

		MainPage = new NavigationPage(new SelectDevicePage());
	}

#if WINDOWS
    // Set window size on Windows system to be similar to mobile screen size
    protected override Window CreateWindow(IActivationState activationState)
    {
        var windowWidth = 400;
        var windowHeight = 800;

        var window = base.CreateWindow(activationState);

		window.Width = windowWidth;
		window.Height = windowHeight;
		
		return window;
    }
#endif
}
