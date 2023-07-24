using RaspiRemote.ViewModels.FileExplorer;

namespace RaspiRemote.Pages.FileExplorer;

public partial class FileEditorPage : ContentPage
{
	public FileEditorPage(string filePath)
	{
		InitializeComponent();

		var viewModel = ServiceHelper.GetService<FileEditorPageViewModel>();
		viewModel.Initialize(filePath);
		BindingContext = viewModel;
    }
}