using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Busybody.Config
{
    public class BusybodyConfig
    {
        public int PollingInterval { get; set; }
        public string DataDirectory { get; set; }
        public EmailAlertConfiguration EmailAlertConfiguration { get; set; }
        public string ListeningUrls { get; set; } 
        public string WebRoot { get; set; } 

        public List<HostConfig> Hosts;

        public BusybodyConfig()
        {
            Hosts = new List<HostConfig>();
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

        public IEnumerable<string> GetListeningUrls()
        {
            if (ListeningUrls == null)
                return new[] {"http://localhost:9000"};
            return ListeningUrls.Split(new []{ ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        static void _SetDefaults(BusybodyConfig config)
        {
            if (config.PollingInterval == 0)
                config.PollingInterval = 60;
            if (config.DataDirectory == null)
                config.DataDirectory = Path.Combine(Path.GetTempPath(), "Busybody");
            if(config.WebRoot == null)
                config.WebRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WebRoot");
        }
    }
}