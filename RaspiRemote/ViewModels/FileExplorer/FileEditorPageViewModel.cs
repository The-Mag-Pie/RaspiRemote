using CommunityToolkit.Mvvm.ComponentModel;
using Renci.SshNet;

namespace RaspiRemote.ViewModels.FileExplorer
{
    internal partial class FileEditorPageViewModel : BaseViewModel
    {
        private readonly SftpClient _sftpClient;
        private string _filePath;

        [ObservableProperty]
        private string _content;

        public FileEditorPageViewModel(SshClientContainer sshClientContainer)
        {
            _sftpClient = sshClientContainer.SftpClient;
        }

        public void Initialize(string path)
        {
            _filePath = path;

            using var stream = _sftpClient.Open(_filePath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(stream);
            Content = reader.ReadToEnd();
        }
    }
}
