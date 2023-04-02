using RaspiRemote.Models;

namespace RaspiRemote.Popups
{
    internal class EditDevicePopup : DevicePopupBase
    {
        public EditDevicePopup(RpiDevice device) 
            : base(new()
        {
            Name = device.Name,
            Host = device.Host,
            Port = device.Port,
            Username = device.Username,
            Password = device.Password
        }, "Edit device") { }
    }
}
