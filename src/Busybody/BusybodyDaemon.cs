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
            var configFilePath = CommonPaths.CurrentConfigFilePath();
            var config = BusybodyConfig.ReadFromFile(configFilePath);
            var eventLogger = new EventLogger();

            foreach (var host in config.Hosts)
            {
                _log.Debug("Checking host " + host.Nickname);
                var allPassed = true;
                foreach (var test in host.Tests)
                {
                    _log.Debug("Running test " + test.Name + " on " + test.HostNickname);
                    switch (test.Name)
                    {
                        case "Ping":
                            var result = _RunPingTest(host);
                            allPassed = allPassed && result;
                            break;
                        default:
                            throw new TestNotFoundException(test.Name);
                    }

                    if (allPassed)
                        eventLogger.Publish("Host: " + host.Nickname + ", State: Up");
                }
            }

            eventLogger.Publish("Startup complete");
        }

        bool _RunPingTest(HostConfig host)
        {
            _log.Debug("Running ping test on host " + host.Nickname + " with hostname " + host.Hostname);
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