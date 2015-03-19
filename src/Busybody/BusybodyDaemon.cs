using System;
using System.Net.NetworkInformation;
using Busybody.Config;

namespace Busybody
{
    public class BusybodyDaemon
    {
        static Logger _log = new Logger(typeof(BusybodyDaemon));

        public void Start()
        {
            var config = AppContext.Instance.Config;
            var eventLogger = new EventLogger();

            foreach (var host in config.Hosts)
            {
                _log.Debug("Checking host " + host.Nickname);
                var allPassed = true;
                var testFactory = new TestFactory();
                foreach (var testConfig in host.Tests)
                {
                    var test = testFactory.Create(testConfig.Name);
                    test.Execute(host, testConfig);

                    if (allPassed)
                        eventLogger.Publish("Host: " + host.Nickname + ", State: Up");
                }
            }

            eventLogger.Publish("Startup complete");
        }
    }
    
    public interface ITestFactory
    {
        IBusybodyTest Create(string name);
    }

    public class TestFactory : ITestFactory
    {
        static Logger _log = new Logger(typeof(BusybodyDaemon));

        public IBusybodyTest Create(string name)
        {
            _log.Debug("Creating test " + name);
            switch (name)
            {
                case "Ping":
                    var result = new PingTest();
                    return result;
                default:
                    throw new TestNotFoundException(name);
            }
        }
    }

    public interface IBusybodyTest
    {
        bool Execute(HostConfig host, HostTestConfig test);
    }

    public class PingTest : IBusybodyTest
    {
        static Logger _log = new Logger(typeof(BusybodyDaemon));

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            var pingConfig = (PingTestConfig) test;
            _log.Debug("Running ping test on host " + pingConfig.HostNickname + " with hostname " + pingConfig.HostNickname);
            var ping = new Ping();
            var result = ping.Send(host.Hostname);
            _log.Debug("Ping status:" + result.Status);
            return result.Status == IPStatus.Success;
        }
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base("Test " + name + " not found.")
        {
        }
   }
}