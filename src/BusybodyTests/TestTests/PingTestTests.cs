using System.Collections.Generic;
using Busybody.Config;
using Busybody.Tests;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests.TestTests
{
    [TestFixture]
    public class PingTestTests
    {
        [Test]
        public void When_pinging_localhost_by_IP()
        {
            var result = _PingHost("127.0.0.1");
            result.Should().BeTrue();
        }

        [Test]
        public void When_pinging_localhost_by_localhost_hostname()
        {
            var result = _PingHost("localhost");
            result.Should().BeTrue();
        }

        [Test]
        public void When_pinging_ip_that_is_not_available()
        {
            var result = _PingHost("10.1.10.1");
            result.Should().BeFalse();
        }

        [Test]
        public void When_pinging_hostname_that_doesnt_exit()
        {
            var result = _PingHost("abcdefghijklmnop");
            result.Should().BeFalse();
        }


        static bool _PingHost(string hostname)
        {
            var hostTestConfig = new HostTestConfig("Ping");
            var hostConfig = new HostConfig
            {
                Hostname = hostname,
                Nickname = "Local machine",
                Tests = new List<HostTestConfig>
                {
                    hostTestConfig,
                },
            };
            hostTestConfig.Parameters.Add("MaxFailures", "0");

            var pingTest = new PingTest();
            var result = pingTest.Execute(hostConfig, hostTestConfig);
            return result;
        }
    }
}
