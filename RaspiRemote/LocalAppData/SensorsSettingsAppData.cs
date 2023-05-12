using RaspiRemote.Models;

namespace RaspiRemote.LocalAppData
{
    class SensorsSettingsAppData : LocalAppDataBase
    {
        private const string SesnorsSettingsFilename = "sensors_settings_{0}.json";
        public static SensorsSettings GetSensorsSettings(string deviceGuid) => _getObject<SensorsSettings>(SesnorsSettingsFilename.Replace("{0}", deviceGuid));
        public static void SaveSensorsSettings(string deviceGuid, SensorsSettings settings) => _saveObject(SesnorsSettingsFilename.Replace("{0}", deviceGuid), settings);
    }
}
