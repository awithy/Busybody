using System.IO;
using Newtonsoft.Json;

namespace BusybodyAgent
{
    public class BusybodyConfig
    {
        public int PollingInterval { get; set; }
        public string DataDirectory { get; set; }

        public BusybodyConfig()
        {
        }

        public void WriteToFile(string filePath)
        {
            var configText = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, configText);
        }

        public static BusybodyConfig ReadFromFile(string filePath)
        {
            var configText = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<BusybodyConfig>(configText);
            _SetDefaults(config);
            return config;
        }

        static void _SetDefaults(BusybodyConfig config)
        {
            if (config.PollingInterval == 0)
                config.PollingInterval = 60;
            if (config.DataDirectory == null)
                config.DataDirectory = Path.Combine(Path.GetTempPath(), "Busybody");
        }
    }
}