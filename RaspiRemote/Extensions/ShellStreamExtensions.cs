using Renci.SshNet;
using System.Reflection;

namespace RaspiRemote.Extensions
{
    public static class ShellStreamExtensions
    {
        /// <summary>
        /// Sends window change request.
        /// </summary>
        /// <returns>true if success, false if failure</returns>
        public static bool SendWindowChangeRequest(this ShellStream shellStream, uint columns, uint rows, uint width, uint height)
        {
            var channel = shellStream.GetType()
                .GetField("_channel", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(shellStream);

            if (channel == null) return false;

            var result = channel.GetType()
                .GetMethod("SendWindowChangeRequest", BindingFlags.Public | BindingFlags.Instance)?
                .Invoke(channel, new object[] { columns, rows, width, height });

            if (result == null) return false;

            return (bool)result;
        }
    }
}
