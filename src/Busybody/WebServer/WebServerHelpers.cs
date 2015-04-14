using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Busybody.WebServer
{
    public static class WebServerHelpers
    {
        public static HttpResponseMessage GetPage(string pageFileName)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(File.ReadAllText(Path.Combine(CommonPaths.WebContentPath(), "..", pageFileName)), Encoding.UTF8, "text/html"),
            };
        }
    }
}
