using Busybody.Config;
using Busybody.Tests;

namespace Busybody
{
    public interface ITestFactory
    {
        IBusybodyTest Create(string name);
    }

    public interface IBusybodyTest
    {
        bool Execute(HostConfig host, HostTestConfig test);
    }

    public class TestFactory : ITestFactory
    {
        static Logger _log = new Logger(typeof(TestFactory));

        public IBusybodyTest Create(string name)
        {
            _log.Trace("Creating test " + name);
            switch (name.ToLower())
            {
                case "ping":
                    return new PingTest();
                case "http":
                    return new HttpTest();
                case "azureagentheartbeat":
                    return new AzureAgentHeartbeatTest();
                case "fileagentheartbeat":
                    return new FileAgentHeartbeatTest();
                default:
                    throw new TestNotFoundException(name);
            }
        }
    }
}