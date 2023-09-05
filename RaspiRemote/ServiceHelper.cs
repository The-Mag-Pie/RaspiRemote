using RaspiRemote.ViewModels;
using RaspiRemote.ViewModels.FileExplorer;
using RaspiRemote.ViewModels.Gpio;

namespace RaspiRemote
{
    public static class ServiceHelper
    {
        private static IServiceProvider _serviceProvider;

        /// <summary>
        /// Set service provider for GetService() method.
        /// </summary>
        public static IServiceProvider ServiceProvider { set { _serviceProvider = value; } }

        /// <summary>
        /// Get service from dependency injection. Throws an exception if specified service cannot be found.
        /// </summary>
        /// <typeparam name="T">A type of the service</typeparam>
        /// <returns>A required service</returns>
        public static T GetService<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        /// Register required services in service collection passed as argument.
        /// </summary>
        /// <param name="serviceCollection">A service collection</param>
        public static void RegisterServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<SshClientContainer>();

            // Register ViewModels
            serviceCollection.AddTransient<SelectDevicePageViewModel>();
            serviceCollection.AddTransient<SensorsPageViewModel>();
            serviceCollection.AddTransient<FileExplorerPageViewModel>();
            serviceCollection.AddTransient<FileEditorPageViewModel>();
            serviceCollection.AddTransient<GpioPageViewModel>();
            serviceCollection.AddTransient<TerminalPageViewModel>();
            serviceCollection.AddTransient<SystemInfoPageViewModel>();
        }
    }
}
