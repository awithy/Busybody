using Busybody.Config;
using Busybody.Tests;
using BusybodyTests.Helpers;

namespace BusybodyTests.Fakes
{
    public class FakeAppContextBuilder
    {
        FakeAppContext _appContext;

        public FakeAppContextBuilder()
        {
            _appContext = new FakeAppContext();
        }

        public FakeAppContextBuilder WithBasicConfiguration()
        {
            var config = new ConfigBuilder()
                .WithHost("Local Machine", "127.0.0.1", "Location 1")
                .WithTest(new HostTestConfig("Ping"))
                .BuildHostConfig()
                .BuildConfig();
            _appContext.Config = config;
            _appContext.FakeTestFactory.Tests.Add("Ping", new FakePingTest());
            return this;
        }

        public FakeAppContextBuilder WithConfig(BusybodyConfig config)
        {
            _appContext.Config = config;
            _appContext.FakeTestFactory.Tests.Add("Ping", new FakePingTest());
            _appContext.FakeTestFactory.Tests.Add("AzureAgentHeartbeat", new AzureAgentHeartbeatTest());
            return this;
        }

        public FakeAppContext Build()
        {
            return _appContext;
        }
    }
}