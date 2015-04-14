using System;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace Busybody.WebServer
{
    [AllowAnonymous]
    public class LoginController : ApiController
    {
        public HttpResponseMessage DoLogin(LoginModel loginModel)
        {
            if (loginModel.Username == "admin" && loginModel.Password == "busybody")
            {
                var context = Request.GetOwinContext();
                var authenticationProperties = new AuthenticationProperties()
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1),
                };
                context.Authentication.SignIn(authenticationProperties,
                    new ClaimsIdentity(new[] {new Claim(ClaimsIdentity.DefaultNameClaimType, loginModel.Username)}, DefaultAuthenticationTypes.ApplicationCookie));
                context.Response.Headers.Add("Location", new[] {"/"});
                return Request.CreateResponse(HttpStatusCode.Found);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Bad username or password"));
            }
        }

        public HttpResponseMessage GetLogin()
        {
            return WebServerHelpers.GetPage("login.html");
        }
    }
}