namespace RaspiRemote.Popups
{
    internal class AddDevicePopup : DevicePopupBase
    {
        public AddDevicePopup() : base(new(GenerateGUID: true), "Add new device") { }
    }
}
