using System.Collections.Generic;
using System.IO;
using Busybody.Config;

namespace BusybodyTests.Helpers
{
    public class ConfigBuilder
    {
        int _pollingInterval = 1;
        public List<HostConfig> Hosts { get; set;  }

        public ConfigBuilder()
        {
            Hosts = new List<HostConfig>();
        }

        public HostConfigBuilder WithHost(string nickname, string hostname)
        {
            var hostConfigBuilder = new HostConfigBuilder(this, nickname, hostname);
            return hostConfigBuilder;
        }

        public ConfigBuilder WithPollingInterval(int pollingInterval)
        {
            _pollingInterval = pollingInterval;
            return this;
        }

        public BusybodyConfig BuildConfig()
        {
            return new BusybodyConfig
            {
                Hosts = Hosts,
                PollingInterval = _pollingInterval,
                DataDirectory = Path.Combine(Path.GetTempPath(), "Busybody"),
            };
        }
    }
}