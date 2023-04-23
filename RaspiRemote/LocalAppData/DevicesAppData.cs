using RaspiRemote.Models;

namespace RaspiRemote.LocalAppData
{
    class DevicesAppData : LocalAppDataBase
    {
        private const string DevicesListFilename = "devices_list.json";
        public static List<RpiDevice> GetDevicesList() => _getObject<List<RpiDevice>>(DevicesListFilename);
        public static void SaveDevicesList(List<RpiDevice> devices) => _saveObject(DevicesListFilename, devices);
    }
}
