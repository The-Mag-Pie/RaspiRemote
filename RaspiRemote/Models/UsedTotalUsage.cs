using CommunityToolkit.Mvvm.ComponentModel;

namespace RaspiRemote.Models
{
    public partial class UsedTotalUsage : ObservableObject
    {
        private (int, int) _usage = (0, 0);

        public (int, int) Usage
        {
            set
            {
                _usage = value;
                OnPropertyChanged(nameof(Used));
                OnPropertyChanged(nameof(Total));
                OnPropertyChanged(nameof(Ratio));
            }
        }

        public int Used => _usage.Item1;
        public int Total => _usage.Item2;
        public double Ratio => (double)Used / (double)Total;
    }
}
