using System.Net.NetworkInformation;
using Busybody.Config;

namespace Busybody
{
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
}