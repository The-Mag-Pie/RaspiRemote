using RaspiRemote.ViewModels;

namespace RaspiRemote.Pages;

public partial class FileExplorerPage : ContentPage
{
	public FileExplorerPage()
	{
		InitializeComponent();

		BindingContext = ServiceHelper.GetService<FileExplorerPageViewModel>();
	}
}