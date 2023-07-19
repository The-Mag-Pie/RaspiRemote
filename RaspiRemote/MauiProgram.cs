using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

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

		// Configure custom handlers
		builder.ConfigureMauiHandlers(handlers =>
		{
#if ANDROID
			handlers.AddHandler<CollectionView, RaspiRemote.Platforms.Android.CustomHandlers.CustomCollectionViewHandler>();
#endif
        });

		// Register services for dependency injection
		ServiceHelper.RegisterServices(builder.Services);

		var app = builder.Build();

		ServiceHelper.ServiceProvider = app.Services;

		return app;
	}
}
