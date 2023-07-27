using RaspiRemote.ViewModels.Gpio;

namespace RaspiRemote.Pages;

public partial class GpioPage : ContentPage
{
	public GpioPage()
	{
		InitializeComponent();

		var vm = ServiceHelper.GetService<GpioPageViewModel>();
		BindingContext = vm;

		Appearing += async (s, e) => await vm.OnAppearing();
	}
}