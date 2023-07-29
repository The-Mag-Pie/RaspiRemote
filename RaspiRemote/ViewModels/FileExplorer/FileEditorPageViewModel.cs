using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace RaspiRemote.ViewModels.FileExplorer
{
    internal partial class FileEditorPageViewModel : BaseViewModel
    {
        private readonly SftpClient _sftpClient;

        [ObservableProperty]
        private SftpFile _file;

        [ObservableProperty]
        private string _content;

        public FileEditorPageViewModel(SshClientContainer sshClientContainer)
        {
            _sftpClient = sshClientContainer.SftpClient;
        }

        public void Initialize(SftpFile file)
        {
            File = file;

            using var stream = _sftpClient.Open(File.FullName, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            Content = reader.ReadToEnd();
        }

        [RelayCommand]
        private async Task Save() => await InvokeAsyncWithLoader(TrySaveFile);

        private async Task TrySaveFile()
        {
            try
            {
                // Convert Windows-style CRLF line ending to Unix-style LF line ending
                Content = Content.Replace("\r\n", "\n");
                Content = Content.Replace("\r", "\n");

                using var stream = _sftpClient.Open(File.FullName, FileMode.Open, FileAccess.Write);
                using var writer = new StreamWriter(stream);
                await writer.WriteAsync(Content);
                await writer.FlushAsync();

                _ = Toast.Make("File has been successfully saved.").Show();
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                _ = DisplayError(ex.Message);
            }
        }

        [RelayCommand]
        private async Task Cancel() =>
            await Application.Current.MainPage.Navigation.PopAsync();
    }
}
