using RaspiRemote.ViewModels.Sensors;

namespace RaspiRemote.Pages;

public partial class SensorsPage : ContentPage
{
	public SensorsPage()
	{
		InitializeComponent();

		var vm = ServiceHelper.GetService<SensorsPageViewModel>();
		BindingContext = vm;

		Appearing += (s, e) => vm.OnAppearing();
		Disappearing += (s, e) => vm.OnDisappearing();
	}
}