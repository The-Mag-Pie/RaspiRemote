using CommunityToolkit.Maui.Alerts;
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
        private readonly SftpClient _sftpClient;
        private readonly Stack<string> _navigationStack = new();

        [ObservableProperty]
        private string _path;

        public bool CanGoBack => _navigationStack.Count > 0;

        public ObservableCollection<SftpFile> Items { get; } = new();

        public FileExplorerPageViewModel(SshClientContainer sshClientContainer)
        {
            _sftpClient = sshClientContainer.SftpClient;

            Path = _sftpClient.WorkingDirectory;

            _ = LoadItems();
        }

        private async Task LoadItems()
        {
            Items.Clear();

            var items = await _sftpClient.ListDirectoryAsync(Path);

            // Sort and show directories first
            var directories = items
                .Where(i => i.IsDirectory)
                .OrderBy(i => i.Name)
                .Skip(1);
            foreach (var dir in directories)
            {
                Items.Add(dir);
            }

            // Sort and show files
            var files = items
                .Where(i => !i.IsDirectory)
                .OrderBy(i => i.Name);
            foreach (var file in files)
            {
                Items.Add(file);
            }
        }

        public async Task GoBack()
        {
            var path = _navigationStack.Pop();
            await InvokeAsyncWithLoader(async () => await TryChangeDirectoryAsync(path));
        }

        [RelayCommand]
        private async Task ItemClicked(SftpFile item)
        {
            await InvokeAsyncWithLoader(async () =>
            {
                if (item.IsDirectory)
                {
                    var oldPath = Path;
                    if (await TryChangeDirectoryAsync($"{Path}/{item.Name}"))
                    {
                        if (item.Name == "..") _navigationStack.TryPop(out var _);
                        else _navigationStack.Push(oldPath);
                    }
                }
                else
                {
                    _ = DisplayAlert("file explorer", $"file clicked: {item.Name}", "ok");
                }
            });
        }

        private async Task<bool> TryChangeDirectoryAsync(string path)
        {
            try
            {
                _sftpClient.ChangeDirectory(path);
                Path = _sftpClient.WorkingDirectory;
                await LoadItems();
                return true;
            }
            catch (Exception ex)
            {
                _ = DisplayAlert("Error", ex.Message, "OK");
                return false;
            }
        }

        [RelayCommand]
        private async Task NewFile()
        {
            var filename = await DisplayPromptAsync("Create new file", "Enter filename", placeholder: "Enter filename here...");
            if (filename is null) return;

            // Check if the provided filename contains illegal characters
            if (filename == "." || filename == ".." || filename.Contains('/'))
            {
                _ = DisplayAlert("Error", "You provided illegal characters.", "OK");
                return;
            }

            // Check if the provided filename already exists
            if (Items.Select(i => i.Name).Contains(filename))
            {
                _ = DisplayAlert("Error", "File or directory with provided name already exists.", "OK");
                return;
            }

            await InvokeAsyncWithLoader(async () => await CreateFile($"{Path}/{filename}"));
        }

        private async Task CreateFile(string path)
        {
            try
            {
                _sftpClient.Create(path);
                await LoadItems();
                _ = Toast.Make("File has been successfully created.").Show();
            }
            catch (Exception ex)
            {
                _ = DisplayAlert("Error", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task NewDirectory()
        {
            var dirname = await DisplayPromptAsync("Create new directory", "Enter directory name", placeholder: "Enter directory name here...");
            if (dirname is null) return;

            // Check if the provided dirname contains illegal characters
            if (dirname == "." || dirname == ".." || dirname.Contains('/'))
            {
                _ = DisplayAlert("Error", "You provided illegal characters.", "OK");
                return;
            }

            // Check if the provided dirname already exists
            if (Items.Select(i => i.Name).Contains(dirname))
            {
                _ = DisplayAlert("Error", "File or directory with provided name already exists.", "OK");
                return;
            }

            await InvokeAsyncWithLoader(async () => await CreateDirectory($"{Path}/{dirname}"));
        }

        private async Task CreateDirectory(string path)
        {
            try
            {
                _sftpClient.CreateDirectory(path);
                await LoadItems();
                _ = Toast.Make("Directory has been successfully created.").Show();
            }
            catch (Exception ex)
            {
                _ = DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}
