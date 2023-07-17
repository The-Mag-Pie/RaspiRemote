using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class FileExplorerPage : ContentPage
{
	public FileExplorerPage()
	{
		InitializeComponent();

		var viewModel = ServiceHelper.GetService<FileExplorerPageViewModel>();
		BindingContext = viewModel;
		Appearing += async (s, e) => await viewModel.OnAppearing();
	}
}