using RaspiRemote.Models;
using System.Text.Json;

namespace RaspiRemote
{
    internal class LocalAppData
    {
        private const string DevicesListFilename = "devices_list.json";

        public static List<RpiDevice> GetDevicesList() => _getObject<List<RpiDevice>>(DevicesListFilename);

        public static void SaveDevicesList(List<RpiDevice> devices) => _saveObject(DevicesListFilename, devices);

        private static string _getFullPath(string filename) => Path.Combine(FileSystem.Current.AppDataDirectory, filename);

        private static T _getObject<T>(string filename) where T : new()
        {
            var fullpath = _getFullPath(filename);

            if (!File.Exists(fullpath))
            {
                return new();
            }

            var fileContents = File.ReadAllText(fullpath);

            if (fileContents.Length == 0)
            {
                return new();
            }

            return JsonSerializer.Deserialize<T>(fileContents);
        }

        private static void _saveObject<T>(string filename, T obj)
        {
            var fullpath = _getFullPath(filename);

            var jsonString = JsonSerializer.Serialize(obj);

            File.WriteAllText(fullpath, jsonString);
        }
    }
}
