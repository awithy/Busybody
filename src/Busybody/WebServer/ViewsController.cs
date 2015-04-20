using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class ViewsController : ApiController
    {
        public HttpResponseMessage GetViews()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        public HttpResponseMessage GetViewById(string id)
        {
            return WebServerHelpers.GetView(id);
        }
    }
}