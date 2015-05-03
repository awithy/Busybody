using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

        public HostConfigBuilder WithHost(string nickname, string hostname, string location)
        {
            var hostConfigBuilder = new HostConfigBuilder(this, nickname, hostname, location);
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
                EmailAlertConfiguration = new EmailAlertConfiguration
                {
                    Enabled = false,
                    FromAddress = "a@a.com",
                    Host = "smtp.doesnotexist",
                    Password = "ABCD",
                    Port = 1234,
                    ToEmailAddress = "b@b.com",
                },
                Hosts = Hosts,
                PollingInterval = _pollingInterval,
                DataDirectory = Path.Combine(Path.GetTempPath(), "Busybody"),
                WebRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\..\\..\\BusybodyWebApp"),
            };
        }
    }
}