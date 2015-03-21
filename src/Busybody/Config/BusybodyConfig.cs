using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Busybody.Config
{
    public class BusybodyConfig
    {
        public List<HostConfig> Hosts = new List<HostConfig>();

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
            return config;
        }
    }
}