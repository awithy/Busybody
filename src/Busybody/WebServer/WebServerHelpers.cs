using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Busybody.WebServer
{
    public static class WebServerHelpers
    {
        public static HttpResponseMessage GetRootPage(string pageFileName)
        {
            var filePath = Path.Combine(CommonPaths.WebRoot(), pageFileName);
            return _GetFile(filePath);
        }

        public static HttpResponseMessage GetView(string viewFileName)
        {
            var filePath = Path.Combine(CommonPaths.WebRoot(), "views", viewFileName);
            return _GetFile(filePath);
        }

        static HttpResponseMessage _GetFile(string filePath)
        {
            if (!File.Exists(filePath))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(File.ReadAllText(filePath), Encoding.UTF8, "text/html"),
            };
        }
    }
}