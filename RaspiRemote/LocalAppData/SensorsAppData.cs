namespace RaspiRemote.LocalAppData
{
    class SensorsAppData : LocalAppDataBase
    {
        private const string DHT11SensorsListFilename = "dht11_sensors_{0}.json";
        public static List<int> GetDHT11SensorsList(string deviceGuid) => _getObject<List<int>>(DHT11SensorsListFilename.Replace("{0}", deviceGuid));
        public static void SaveDHT11SensorsList(string deviceGuid, List<int> sensorsList) => _saveObject(DHT11SensorsListFilename.Replace("{0}", deviceGuid), sensorsList);
    }
}
