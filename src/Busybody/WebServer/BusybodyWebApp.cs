using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Busybody.WebServer
{
    public class BusybodyWebApp
    {
        static Logger _log = new Logger(typeof(BusybodyWebApp));

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

            var webContentPath = CommonPaths.WebContentPath();
            _log.Debug("Starting web app with web content path " + webContentPath);

            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = new PathString("/webroot/webcontent"),
                FileSystem = new PhysicalFileSystem(webContentPath),
            });

            app.UseWebApi(config);
        }
    }
}