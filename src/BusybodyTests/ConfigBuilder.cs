using System.Collections.Generic;
using Busybody.Config;

namespace BusybodyTests
{
    public class ConfigBuilder
    {
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

        public BusybodyConfig BuildConfig()
        {
            return new BusybodyConfig
            {
                Hosts = Hosts,
            };
        }
    }
}