using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SelectDevicePage : ContentPage
{
	public SelectDevicePage(SshClientContainer sshClientContainer)
	{
		InitializeComponent();

		BindingContext = new SelectDevicePageViewModel(sshClientContainer);
	}
}