using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace RaspiRemote.ViewModels
{
    internal partial class FileExplorerPageViewModel : BaseViewModel
    {
        public ObservableCollection<string> Items { get; } = new();

        public FileExplorerPageViewModel(SshClientContainer sshClientContainer)
        {
            for (int i = 0; i < 10; i++)
            {
                Items.Add($"some file {i}");
            }
        }

        [RelayCommand]
        private void TestCmd()
        {
            Application.Current.MainPage.DisplayAlert("xd", "good", "ok");
            Items.Add("xd");
        }
    }
}
