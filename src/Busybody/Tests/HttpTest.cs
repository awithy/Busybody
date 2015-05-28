using System;
using System.Collections.Generic;
using System.Net;
using Busybody.Config;
using BusybodyShared;

namespace Busybody.Tests
{
    public class HttpTest : IBusybodyTest
    {
        static readonly Logger _log = new Logger(typeof(HttpTest));

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            var parameters = new HttpTestParameters(test.Parameters);
            _log.TraceFormat("Running http test on Host: {0}, Hostname: {1}, {2}", host.Nickname, host.Hostname, parameters.ToLogString());

            var hostUri = new Uri("http://" + host.Hostname);
            var request = (HttpWebRequest)HttpWebRequest.Create(hostUri);
            request.Timeout = parameters.TimeoutMs;
            request.Method = "GET";
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                    return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _log.TraceFormat("Exception thrown while executing http test on {0}.", hostUri);
                return false;
            }
        }

        private class HttpTestParameters
        {
            public int TimeoutMs { get; set; }

            public HttpTestParameters(Dictionary<string,string> parameters)
            {
                TimeoutMs = _ParseIntFromDictionary(parameters, "TimeoutMs", 2000);
            }

            public string ToLogString()
            {
                return string.Format("TimeoutMs:{0}", TimeoutMs);
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