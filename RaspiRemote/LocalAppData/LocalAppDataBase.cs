using System.Text.Json;

namespace RaspiRemote.LocalAppData
{
    class LocalAppDataBase
    {
        private static string _getFullPath(string filename) => Path.Combine(FileSystem.Current.AppDataDirectory, filename);

        protected static T _getObject<T>(string filename) where T : new()
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

        protected static void _saveObject<T>(string filename, T obj)
        {
            var fullpath = _getFullPath(filename);

            var jsonString = JsonSerializer.Serialize(obj);

            File.WriteAllText(fullpath, jsonString);
        }
    }
}
