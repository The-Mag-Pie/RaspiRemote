using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SystemInfoPage : ContentPage
{
	public SystemInfoPage()
	{
		InitializeComponent();

		BindingContext = ServiceHelper.GetService<SystemInfoPageViewModel>();
	}
}