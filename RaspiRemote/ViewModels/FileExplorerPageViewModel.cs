using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Extensions;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.Collections.ObjectModel;

namespace RaspiRemote.ViewModels
{
    internal partial class FileExplorerPageViewModel : BaseViewModel
    {
        private SftpClient _sftpClient;

        [ObservableProperty]
        private string _path;

        public ObservableCollection<SftpFile> Items { get; } = new();

        public FileExplorerPageViewModel(SshClientContainer sshClientContainer)
        {
            _sftpClient = sshClientContainer.SftpClient;

            Path = _sftpClient.WorkingDirectory;
        }

        public async Task OnAppearing()
        {
            await LoadItems();
        }

        private async Task LoadItems()
        {
            Items.Clear();

            var items = await _sftpClient.ListDirectoryAsync(Path);

            // Sort and show directories first
            var directories = items.Where(i => i.IsDirectory).OrderBy(i => i.Name);
            foreach (var dir in directories)
            {
                Items.Add(dir);
            }

            // Sort and show files
            var files = items.Where(i => !i.IsDirectory).OrderBy(i => i.Name);
            foreach (var file in files)
            {
                Items.Add(file);
            }
        }

        [RelayCommand]
        private void TestCmd()
        {
            Application.Current.MainPage.DisplayAlert("xd", "good", "ok");
            //Items.Add("xd");
        }
    }
}
