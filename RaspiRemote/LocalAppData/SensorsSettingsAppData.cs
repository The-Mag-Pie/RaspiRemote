using RaspiRemote.Models;

namespace RaspiRemote.LocalAppData
{
    class SensorsSettingsAppData : LocalAppDataBase
    {
        private const string SesnorsSettingsFilename = "sensors_settings.json";
        public static SensorsSettings GetSensorsSettings() => _getObject<SensorsSettings>(SesnorsSettingsFilename);
        public static void SaveSensorsSettings(SensorsSettings settings) => _saveObject(SesnorsSettingsFilename, settings);
    }
}
