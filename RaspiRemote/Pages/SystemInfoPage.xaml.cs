using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SystemInfoPage : ContentPage
{
	public SystemInfoPage()
	{
		InitializeComponent();

		var vm = ServiceHelper.GetService<SystemInfoPageViewModel>();
		BindingContext = vm;

		Appearing += (s, e) => vm.OnAppearing();
		Disappearing += (s, e) => vm.OnDisappearing();
	}
}