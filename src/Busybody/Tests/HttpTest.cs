using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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

            var hostUri = parameters.Uri ?? new Uri("http://" + host.Hostname);
            var request = (HttpWebRequest)WebRequest.Create(hostUri);
            request.Timeout = parameters.TimeoutMs;
            request.Method = "GET";
            try

            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        return false;

                    if (parameters.SearchString == null)
                        return true;

                    var responseString = _GetResponseString(response);

                    if (responseString.Contains(parameters.SearchString))
                        return true;
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.TraceFormat("Exception thrown while executing http test on {0}.  Detail:{1}", hostUri, ex);
                return false;
            }
        }

        static string _GetResponseString(HttpWebResponse response)
        {
            var sb = new StringBuilder();
            int count;
            var buf = new byte[8192];
            var responseStream = response.GetResponseStream();
            do
            {
                count = responseStream.Read(buf, 0, buf.Length);
                if (count != 0)
                {
                    var tempString = Encoding.ASCII.GetString(buf, 0, count);
                    sb.Append(tempString);
                }
            } while (count > 0);

            var responseString = sb.ToString();
            return responseString;
        }

        private class HttpTestParameters
        {
            public int TimeoutMs { get; set; }
            public string SearchString { get; set; }
            public Uri Uri { get; set; }

            public HttpTestParameters(Dictionary<string,string> parameters)
            {
                TimeoutMs = _ParseIntFromDictionary(parameters, "TimeoutMs", 2000);
                if(parameters.ContainsKey("SearchString"))
                    SearchString = parameters["SearchString"];
                if(parameters.ContainsKey("Uri"))
                    Uri = new Uri(parameters["Uri"]);
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