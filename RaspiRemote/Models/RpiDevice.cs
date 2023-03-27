namespace RaspiRemote.Models
{
    internal class RpiDevice
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; } = 22; // Default SSH port is 22
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
