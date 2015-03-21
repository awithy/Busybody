using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Busybody.Config
{
    public class BusybodyConfig
    {
        public int PollingInterval { get; set; }
        public List<HostConfig> Hosts;

        public BusybodyConfig()
        {
            Hosts = new List<HostConfig>();
        }

        public void WriteToFile(string filePath)
        {
            var configText = JsonConvert.SerializeObject(this);
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
        }
    }
}