using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote
{
    public class SshClientContainer
    {
        public RpiDevice DeviceInfo { get; private set; }
        public SshClient SshClient { get; private set; }

        public async Task SetDataAndConnectAsync(RpiDevice deviceInfo)
        {
            DeviceInfo = deviceInfo;

            // Disconnect device if already connected
            if (SshClient != null && SshClient.IsConnected)
            {
                SshClient.Disconnect();
                SshClient.Dispose();
            }

            SshClient = new SshClient(DeviceInfo.Host, DeviceInfo.Port, DeviceInfo.Username, DeviceInfo.Password);
            await Task.Run(SshClient.Connect);
            if (SshClient.IsConnected == false)
            {
                throw new Renci.SshNet.Common.SshConnectionException("Device not connected");
            }
        }
    }
}
