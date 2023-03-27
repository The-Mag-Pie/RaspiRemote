using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class SelectDevicePage : ContentPage
{
	private SelectDevicePageViewModel _viewModel;

	public SelectDevicePage()
	{
		InitializeComponent();

		_viewModel = new();
		BindingContext = _viewModel;
	}
}