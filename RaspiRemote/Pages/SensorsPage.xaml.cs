using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SensorsPage : ContentPage
{
	public SensorsPage(SshClientContainer sshClientContainer)
	{
		InitializeComponent();

		var vm = new SensorsPageViewModel(sshClientContainer);
		BindingContext = vm;

		Appearing += (s, e) => vm.OnAppearing();
	}
}