using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using Busybody.Config;
using Busybody.Utility;

namespace Busybody.Tests
{
    public class PingTest : IBusybodyTest
    {
        static readonly Logger _log = new Logger(typeof(PingTest));

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            var parameters = new PingTestParameters(test.Parameters);
            _log.TraceFormat("Running ping test on Host: {0}, Hostname: {1}, {2}", host.Nickname, host.Hostname, parameters.ToLogString());

            var failures = 0;
            for (var cnt = 0; cnt < parameters.Count; cnt++)
            {
                _log.TraceFormat("Executing ping test {0} of {1}.  Current failures:{2}.  Max failures:{3}", cnt, parameters.Count, failures, parameters.MaxFailures);
                var success = false;
                try
                {
                    var result = new Ping().Send(host.Hostname);
                    if(result != null)
                        success = result.Status == IPStatus.Success && result.RoundtripTime <= parameters.TimeoutMs;
                    _log.TraceFormat("Ping result received for host {0}. Status:{1}, RoundtripMs:{2}", host.Nickname, result.Status, result.RoundtripTime);
                }
                catch (PingException ex)
                {
                    _log.TraceFormat("PingException occurred while pinging host {0} with detail: {1}", host.Nickname, ex);
                }

                if (!success)
                    failures++;
                if (failures > parameters.MaxFailures)
                {
                    _log.TraceFormat("Failing ping text because failures > max failures");
                    return false;
                }

                Thread.Sleep(parameters.DelayMs);
            }
            _log.TraceFormat("Ping test successful");
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
                TimeoutMs = _ParseIntFromDictionary(parameters, "TimeoutMs", 2000);
                Count = _ParseIntFromDictionary(parameters, "Count", 5);
                MaxFailures = _ParseIntFromDictionary(parameters, "MaxFailures", 1);
                DelayMs = _ParseIntFromDictionary(parameters, "DelayMs", 500);
            }

            public string ToLogString()
            {
                return string.Format("Timeout:{0}, Count:{1}, MaxFailures:{2}, Delay:{3}", TimeoutMs, Count, MaxFailures, DelayMs);
            }

            int _ParseIntFromDictionary(Dictionary<string, string> parameters, string paramName, int defaultValue)
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