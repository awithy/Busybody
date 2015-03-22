using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Busybody.Config;

namespace Busybody.Tests
{
    public class PingTest : IBusybodyTest
    {
        static readonly Logger _log = new Logger(typeof(BusybodyDaemon));

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            _log.Debug("Running ping test on host " + host.Nickname + " with hostname " + host.Hostname);
            var parameters = new PingTestParameters(test.Parameters);
            _log.Debug(parameters.ToLogString());

            var failures = 0;
            for (var cnt = 0; cnt < parameters.Count; cnt++)
            {
                var ping = new Ping();
                var result = ping.Send(host.Hostname);
                _log.Debug(string.Format("Ping result received. Status:{0}, RoundtripMs:{1}", result.Status, result.RoundtripTime));
                var success = result.Status == IPStatus.Success;
                success = success && result.RoundtripTime <= parameters.TimeoutMs;
                if (!success)
                    failures++;
                if (failures > parameters.MaxFailures)
                    return false;
                Thread.Sleep(parameters.DelayMs);
            }
            return true;
        }

        private class PingTestParameters
        {
            public int TimeoutMs { get; set; }
            public int Count { get; set; }
            public int MaxFailures { get; set; }
            public int DelayMs { get; set; }

            public PingTestParameters(Dictionary<string,string> parameters)
            {
                TimeoutMs = _ParseInt(parameters, "TimeoutMs", 2000);
                Count = _ParseInt(parameters, "Count", 1);
                MaxFailures = _ParseInt(parameters, "MaxFailures", 1);
                DelayMs = _ParseInt(parameters, "DelayMs", 500);
            }

            public string ToLogString()
            {
                return string.Format("Timeout:{0}, Count:{1}, MaxFailures:{2}, Delay:{3}", TimeoutMs, Count, MaxFailures, DelayMs);
            }

            int _ParseInt(Dictionary<string, string> parameters, string paramName, int defaultValue)
            {
                var timeoutParam = parameters.GetValueOrNullIgnoreCase(paramName);
                if (timeoutParam != null)
                {
                    int tryTimeout = 0;
                    var result = int.TryParse(timeoutParam, out tryTimeout);
                    if(!result)
                        _log.Warn(string.Format("Unable to parse ping test parameter: {0}. Using default value.", paramName));
                    return result ? tryTimeout : defaultValue;
                }
                return defaultValue;
            }
        }
    }
}