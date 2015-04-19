using System.Collections.Generic;
using Busybody.Config;

namespace BusybodyTests.Helpers
{
    public class HostConfigBuilder
    {
        readonly ConfigBuilder _configBuilder;
        readonly string _nickname;
        readonly string _hostname;
        readonly string _location;
        List<HostTestConfig> Tests { get; set; }

        public HostConfigBuilder(ConfigBuilder configBuilder, string nickname, string hostname, string location)
        {
            Tests = new List<HostTestConfig>();
            _configBuilder = configBuilder;
            _nickname = nickname;
            _hostname = hostname;
            _location = location;
        }

        public ConfigBuilder BuildHostConfig()
        {
            var hostConfig = new HostConfig
            {
                Nickname = _nickname,
                Hostname = _hostname,
                Location = _location,
                Tests = Tests,
            };
            _configBuilder.Hosts.Add(hostConfig);
            return _configBuilder;
        }

        public HostConfigBuilder WithTest(HostTestConfig hostTestConfig)
        {
            Tests.Add(hostTestConfig);
            return this;
        }
    }
}