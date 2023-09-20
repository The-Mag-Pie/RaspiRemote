using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RaspiRemote.Helpers;
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

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RAMUsageRatio))]
        private (int, int) _RAMUsageMegabytes;
        public double RAMUsageRatio => (double)RAMUsageMegabytes.Item1 / (double)RAMUsageMegabytes.Item2;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SwapUsageRatio))]
        private (int, int) _SwapUsageMegabytes;
        public double SwapUsageRatio => (double)SwapUsageMegabytes.Item1 / (double)SwapUsageMegabytes.Item2;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RootPartitionUsageRatio))]
        private (int, int) _RootPartitionUsageMegabytes;
        public double RootPartitionUsageRatio => (double)RootPartitionUsageMegabytes.Item1 / (double)RootPartitionUsageMegabytes.Item2;

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
                RAMUsageMegabytes = SystemInfoHelpers.GetRAMUsage(_sshClient);
                SwapUsageMegabytes = SystemInfoHelpers.GetSwapUsage(_sshClient);
                RootPartitionUsageMegabytes = SystemInfoHelpers.GetRootPartitionUsage(_sshClient);
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
