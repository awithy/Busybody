using System.Collections.Generic;
using Busybody.Config;

namespace BusybodyTests
{
    public class HostConfigBuilder
    {
        readonly ConfigBuilder _configBuilder;
        readonly string _nickname;
        readonly string _hostname;
        List<HostTestConfig> Tests { get; set; }

        public HostConfigBuilder(ConfigBuilder configBuilder, string nickname, string hostname)
        {
            Tests = new List<HostTestConfig>();
            _configBuilder = configBuilder;
            _nickname = nickname;
            _hostname = hostname;
        }

        public ConfigBuilder BuildHostConfig()
        {
            var hostConfig = new HostConfig
            {
                Nickname = _nickname,
                Hostname = _hostname,
                Tests = Tests,
            };
            _configBuilder.Hosts.Add(hostConfig);
            return _configBuilder;
        }

        public HostConfigBuilder WithTest(HostTestConfig hostTestConfig)
        {
            hostTestConfig.HostNickname = _nickname;
            Tests.Add(hostTestConfig);
            return this;
        }
    }
}