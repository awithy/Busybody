using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class HomeController : ApiController
    {
        [Authorize]
        public HttpResponseMessage GetHome()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(File.ReadAllText(Path.Combine(CommonPaths.WebContentPath(), "..", "Index.html")), Encoding.UTF8, "text/html"),
            };
        }
    }
}