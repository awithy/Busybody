using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class TemplatesController : ApiController
    {
        public HttpResponseMessage GetTemplates()
        {
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        public HttpResponseMessage GetTemplateById(string id)
        {
            return WebServerHelpers.GetTemplate(id);
        }
    }
}