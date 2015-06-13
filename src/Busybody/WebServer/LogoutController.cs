using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class LogoutController : ApiController
    {
        public HttpResponseMessage DoLogout()
        {
            var context = Request.GetOwinContext();
            context.Authentication.SignOut();
            return Request.CreateResponse(HttpStatusCode.OK, new {success = true});
        }
    }
}
