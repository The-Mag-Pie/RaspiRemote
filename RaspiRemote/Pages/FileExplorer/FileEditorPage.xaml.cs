using RaspiRemote.ViewModels.FileExplorer;
using Renci.SshNet.Sftp;

namespace RaspiRemote.Pages.FileExplorer;

public partial class FileEditorPage : ContentPage
{
	public FileEditorPage(SftpFile file)
	{
		InitializeComponent();

		if (file.IsRegularFile is false)
			throw new Renci.SshNet.Common.SshException("Cannot open this file");

		var viewModel = ServiceHelper.GetService<FileEditorPageViewModel>();
		viewModel.Initialize(file);
		BindingContext = viewModel;
    }
}