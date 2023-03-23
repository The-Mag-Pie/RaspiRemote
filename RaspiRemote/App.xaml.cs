namespace RaspiRemote;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Force light theme (disable dark theme)
		Current.UserAppTheme = AppTheme.Light;

		MainPage = new AppShell();
	}
}
