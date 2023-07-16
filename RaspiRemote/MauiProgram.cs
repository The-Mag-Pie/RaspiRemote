using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using RaspiRemote.Pages;

namespace RaspiRemote;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		// Register services for dependency injection
		ServiceHelper.RegisterServices(builder.Services);

		var app = builder.Build();

		ServiceHelper.ServiceProvider = app.Services;

		return app;
	}
}
