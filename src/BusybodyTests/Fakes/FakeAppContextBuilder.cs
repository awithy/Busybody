using Busybody.Config;

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
                .WithHost("Local Machine", "127.0.0.1")
                .WithTest(new PingTestConfig())
                .BuildHostConfig()
                .BuildConfig();
            _appContext.Config = config;
            _appContext.FakeTestFactory.Tests.Add("Ping", new FakePingTest());
            return this;
        }

        public FakeAppContext Build()
        {
            return _appContext;
        }
    }
}