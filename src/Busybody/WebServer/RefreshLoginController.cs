using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Busybody.WebServer
{
    public class RefreshLoginController : ApiController
    {
        public HttpResponseMessage DoRefresh()
        {
            var context = Request.GetOwinContext();
            if (context.Authentication.User != null && context.Authentication.User.Identities.Any())
            {
                return Request.CreateResponse(HttpStatusCode.OK, new {success = true});
            }
            return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Bad username or password"));
        }
    }
}
