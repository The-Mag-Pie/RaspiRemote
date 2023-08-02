using RaspiRemote.Models;
using Renci.SshNet;

namespace RaspiRemote
{
    public class SshClientContainer : IDisposable
    {
        /// <summary>
        /// Information about connected device
        /// </summary>
        public RpiDevice DeviceInfo { get; private set; }

        /// <summary>
        /// SSH client
        /// </summary>
        public SshClient SshClient { get; private set; }

        /// <summary>
        /// SFTP client
        /// </summary>
        public SftpClient SftpClient { get; private set; }

        /// <summary>
        /// Saves data about device and try to connect to it
        /// </summary>
        /// <param name="deviceInfo">Information about device</param>
        public async Task SetDataAndConnectAsync(RpiDevice deviceInfo)
        {
            DeviceInfo = deviceInfo;

            await ConnectSshClient(deviceInfo);
            await ConnectSftpClient(deviceInfo);
        }

        public async Task ReconnectToCurrentDevice()
        {
            await ConnectSshClient(DeviceInfo);
            await ConnectSftpClient(DeviceInfo);
        }

        private async Task ConnectSshClient(RpiDevice deviceInfo)
        {
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

        private async Task ConnectSftpClient(RpiDevice deviceInfo)
        {
            // Disconnect device if already connected
            if (SftpClient != null && SftpClient.IsConnected)
            {
                SftpClient.Disconnect();
                SftpClient.Dispose();
            }

            SftpClient = new SftpClient(DeviceInfo.Host, DeviceInfo.Port, DeviceInfo.Username, DeviceInfo.Password);
            await Task.Run(SftpClient.Connect);
            if (SftpClient.IsConnected == false)
            {
                throw new Renci.SshNet.Common.SshConnectionException("Device not connected");
            }
        }

        /// <summary>
        /// Disconnect device and release resources
        /// </summary>
        public void Dispose()
        {
            SshClient?.Disconnect();
            SshClient?.Dispose();

            SftpClient?.Disconnect();
            SftpClient?.Dispose();
        }
    }
}
