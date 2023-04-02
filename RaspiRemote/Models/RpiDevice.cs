using CommunityToolkit.Mvvm.ComponentModel;

namespace RaspiRemote.Models
{
    public partial class RpiDevice : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _host;

        [ObservableProperty]
        private int _port = 22; // Default SSH port is 22

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;
    }
}
