using System;
using System.IO;
using BusybodyShared;
using Newtonsoft.Json;

namespace BusybodyAgent
{
    public class BusybodyAgentConfig
    {
        public string AgentId { get; set; }
        public int PollingInterval { get; set; }
        public string DataDirectory { get; set; }
        public string AgentChannelType { get; set; }
        public AzureAgentChannelConfig AzureAgentChannelConfig { get; set; }
        public FileAgentChannelConfig FileAgentChannelConfig { get; set; }

        public void WriteToFile(string filePath)
        {
            var configText = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(filePath, configText);
        }

        public static BusybodyAgentConfig ReadFromFile(string filePath)
        {
            var configText = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<BusybodyAgentConfig>(configText);
            _SetDefaults(config);
            return config;
        }

        static void _SetDefaults(BusybodyAgentConfig config)
        {
            if (config.PollingInterval == 0)
                config.PollingInterval = 60;
            if (config.DataDirectory == null)
                config.DataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Busybody");
        }
    }
}