using Renci.SshNet;
using Renci.SshNet.Common;

namespace RaspiRemote.Helpers
{
    public struct SystemInfoCommands
    {
        public const string Model = "cat /proc/cpuinfo | grep Model | awk -F ': ' '{print $2}'";
        public const string Architecture = "uname -m";
        public const string OSName = "cat /etc/os-release | grep -v PRETTY_NAME | grep NAME | awk -F '\"' '{print $2}'";
        public const string OSVersion = "cat /etc/os-release | grep -v VERSION_ | grep VERSION | awk -F '\"' '{print $2}'";
        public const string Kernel = "echo \"$(uname -rs)\" \"(\"\"$(getconf LONG_BIT)\" \"bit)\"";
        public const string Hostname = "hostname";
        public const string IPv4Addresses = "ip add show | grep -v \"inet 127.0.0.1\" | grep -v inet6 | grep inet | awk -F ' ' '{print $2}'";
        public const string IPv6Addresses = "ip add show | grep -v \"inet6 ::1\" | grep inet6 | awk -F ' ' '{print $2}'";
    }

    public static class SystemInfoHelpers
    {
        public static string GetModel(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.Model);
        public static string GetArchitecture(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.Architecture);
        public static string GetOSName(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.OSName);
        public static string GetOSVersion(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.OSVersion);
        public static string GetKernel(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.Kernel);
        public static string GetHostname(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.Hostname);
        public static string GetIPv4Addresses(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.IPv4Addresses);
        public static string GetIPv6Addresses(SshClient sshClient) => ExecuteCommand(sshClient, SystemInfoCommands.IPv6Addresses);

        private static string ExecuteCommand(SshClient sshClient, string commandText)
        {
            var command = sshClient.RunCommand(commandText);

            if (command.ExitStatus != 0 && command.Error.Length > 0)
                throw new SshException(command.Error);
            else if (command.ExitStatus != 0 && command.Result.Length > 0)
                throw new SshException(command.Result);
            else if (command.ExitStatus != 0)
                throw new SshException("Error during loading data.");
            else
                return command.Result.Trim();
        }
    }
}
