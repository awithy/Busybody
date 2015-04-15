using System.Net.Http;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class HomeController : ApiController
    {
        public HttpResponseMessage GetHome()
        {
            return WebServerHelpers.GetPage("index.html");
        }
    }
}