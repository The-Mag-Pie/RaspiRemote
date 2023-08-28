using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace RaspiRemote.Extensions
{
    public static class SftpClientExtensions
    {
        /// <summary>
        /// <inheritdoc cref="SftpClient.ListDirectory(string, Action{int})"/>
        /// </summary>
        public static async Task<IEnumerable<SftpFile>> ListDirectoryAsync(this SftpClient sftpClient, string path, Action<int> listCallback = null)
        {
            return await Task.Run(() => sftpClient.ListDirectory(path, listCallback));
        }
    }
}
