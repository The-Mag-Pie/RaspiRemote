using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SelectDevicePage : ContentPage
{
	public SelectDevicePage()
	{
		InitializeComponent();

		BindingContext = ServiceHelper.GetService<SelectDevicePageViewModel>();
	}
}