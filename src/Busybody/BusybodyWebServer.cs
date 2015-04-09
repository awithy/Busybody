using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Busybody
{
    public class BusybodyWebServer
    {
        IDisposable _webApp;

        public void Start()
        {
            var startOptions = new StartOptions();
            foreach(var listeningUrl in AppContext.Instance.Config.GetListeningUrls())
                startOptions.Urls.Add(listeningUrl);
            _webApp = WebApp.Start<BusybodyWebApp>(startOptions);
        }

        public void Stop()
        {
            _webApp.Dispose();
        }
    }

    public class BusybodyWebApp
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                LoginPath = new PathString("/login"),
                AuthenticationMode = AuthenticationMode.Active,
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                CookieHttpOnly = true,
                CookieSecure = CookieSecureOption.SameAsRequest,
                CookiePath = "/",
            });

            var config = new HttpConfiguration(); 
            config.Routes.MapHttpRoute( 
                name: "DefaultApi", 
                routeTemplate: "{controller}/{id}",
                defaults: new { controller="home", id = RouteParameter.Optional } 
            );

            var webContentPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "webcontent");
            WebContentPath.Path = webContentPath; //obviously temporary

            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = new PathString("/webcontent"),
                FileSystem = new PhysicalFileSystem(webContentPath),
            });

            app.UseWebApi(config);
        }
    }

    public static class WebContentPath
    {
        public static string Path { get; set; }
    }

    public class HostModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string LastUpdate { get; set; }
        public string LastStateChange { get; set; }
    }

    public class HostsController : ApiController
    {
        [Authorize]
        public IEnumerable<HostModel> GetHosts()
        {
            var hosts = AppContext.Instance.HostRepository.GetHosts();
            var hostModels = hosts.Select(x => new HostModel
            {
                Name = x.Name,
                State = x.State.ToString(),
                LastUpdate = x.LastUpdate.ToString("yyyy-MM-dd HH:mm:ss"),
                LastStateChange = x.LastStateChange.ToString("yyyy-MM-dd HH:mm:ss"),
            });
            return hostModels;
        }
    }

    public static class CookieManager
    {
        public static string Guid { get; set; }
    }

    [Authorize]
    public class HomeController : ApiController
    {
        [Authorize]
        public HttpResponseMessage GetHome()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(File.ReadAllText(Path.Combine(WebContentPath.Path, "..", "Index.html")), Encoding.UTF8, "text/html"),
            };
        }
    }

    [AllowAnonymousAttribute]
    public class LoginController : ApiController
    {
        public HttpResponseMessage DoLogin(LoginModel loginModel)
        {
            var context = Request.GetOwinContext();
            var authenticationProperties = new AuthenticationProperties()
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddDays(1),
            };
            context.Authentication.SignIn(authenticationProperties,
                new ClaimsIdentity(new[] {new Claim(ClaimsIdentity.DefaultNameClaimType, loginModel.Username)}, DefaultAuthenticationTypes.ApplicationCookie));
            context.Response.Headers.Add("Location", new []{ "/" });
            CookieManager.Guid = Guid.NewGuid().ToString("N");
            context.Response.Cookies.Append("authcookie", CookieManager.Guid);
            return Request.CreateResponse(HttpStatusCode.Found);
        }

        public HttpResponseMessage GetLogin()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(File.ReadAllText(Path.Combine(WebContentPath.Path, "..", "login.html")), Encoding.UTF8, "text/html"),
            };
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}