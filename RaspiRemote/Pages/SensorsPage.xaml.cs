using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SensorsPage : ContentPage
{
	public SensorsPage(SshClientContainer sshClientContainer)
	{
		InitializeComponent();
		BindingContext = new SensorsPageViewModel(sshClientContainer);
	}
}