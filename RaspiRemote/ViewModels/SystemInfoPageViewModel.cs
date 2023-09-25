using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Helpers;
using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote.ViewModels
{
    internal partial class SystemInfoPageViewModel : BaseViewModel
    {
        private readonly SshClient _sshClient;
        private CancellationTokenSource _cts;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _model;

        [ObservableProperty]
        private string _architecture;

        [ObservableProperty]
        private string _OSName;

        [ObservableProperty]
        private string _OSVersion;

        [ObservableProperty]
        private string _kernel;

        [ObservableProperty]
        private string _hostname;

        [ObservableProperty]
        private string _IPv4Addresses;

        [ObservableProperty]
        private string _IPv6Addresses;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CPUUsagePercent))]
        private double _CPUUsageRatio;
        public double CPUUsagePercent => Math.Round(CPUUsageRatio * 100, 1);

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CPUTemperatureDeg))]
        private double _CPUTemperatureRatio;
        public double CPUTemperatureDeg => Math.Round(CPUTemperatureRatio * 100, 1);

        public UsedTotalUsage RAMUsage { get; } = new();

        public UsedTotalUsage SwapUsage { get; } = new();

        public UsedTotalUsage RootPartitionUsage { get; } = new();

        public void OnAppearing()
        {
            if (_cts is not null && _cts.IsCancellationRequested is false)
                OnDisappearing();

            _cts = new CancellationTokenSource();
            
            Task.Run(UpdateLiveData);
        }

        public void OnDisappearing() => _cts.Cancel();

        public SystemInfoPageViewModel(SshClientContainer sshClientContainer)
        {
            _sshClient = sshClientContainer.SshClient;

            _ = InvokeAsyncWithLoader(LoadBasicInfo);
        }

        private async Task LoadBasicInfo() => await Task.Run(() =>
        {
            Model = SystemInfoHelpers.GetModel(_sshClient);
            Architecture = SystemInfoHelpers.GetArchitecture(_sshClient);
            OSName = SystemInfoHelpers.GetOSName(_sshClient);
            OSVersion = SystemInfoHelpers.GetOSVersion(_sshClient);
            Kernel = SystemInfoHelpers.GetKernel(_sshClient);
            Hostname = SystemInfoHelpers.GetHostname(_sshClient);
            IPv4Addresses = SystemInfoHelpers.GetIPv4Addresses(_sshClient);
            IPv6Addresses = SystemInfoHelpers.GetIPv6Addresses(_sshClient);
        });

        private void UpdateLiveData()
        {
            while (_cts.IsCancellationRequested is false)
            {
                CPUUsageRatio = SystemInfoHelpers.GetCPUUsage(_sshClient);
                CPUTemperatureRatio = SystemInfoHelpers.GetCPUTemperature(_sshClient);
                RAMUsage.Usage = SystemInfoHelpers.GetRAMUsage(_sshClient);
                SwapUsage.Usage = SystemInfoHelpers.GetSwapUsage(_sshClient);
                RootPartitionUsage.Usage = SystemInfoHelpers.GetRootPartitionUsage(_sshClient);
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            IsRefreshing = true;
            await LoadBasicInfo();
            IsRefreshing = false;
        }
    }
}
