using System.Collections.Generic;

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
            var configFileWriter = new ConfigFileWriter();
            configFileWriter.Write(this, filePath);
        }

        public static BusybodyConfig ReadFromFile(string filePath)
        {
            var configFileReader = new ConfigFileReader();
            var config = configFileReader.ReadFromFile(filePath);
            return config;
        }
    }
}