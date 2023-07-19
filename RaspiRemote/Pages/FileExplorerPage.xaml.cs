using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class FileExplorerPage : ContentPage
{
	private FileExplorerPageViewModel _viewModel;

	public FileExplorerPage()
	{
		InitializeComponent();

		_viewModel = ServiceHelper.GetService<FileExplorerPageViewModel>();

		BindingContext = _viewModel;
    }

    protected override bool OnBackButtonPressed()
    {
		if (_viewModel.CanGoBack)
		{
			_ = _viewModel.GoBack();
			return true;
		}

        return base.OnBackButtonPressed();
    }
}