using System;
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
                foreach (var testConfig in host.Tests)
                {
                    var test = AppContext.Instance.TestFactory.Create(testConfig.Name);
                    test.Execute(host, testConfig);

                    if (allPassed)
                        eventLogger.Publish("Host: " + host.Nickname + ", State: Up");
                }
            }

            eventLogger.Publish("Startup complete");
        }
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base("Test " + name + " not found.")
        {
        }
   }
}