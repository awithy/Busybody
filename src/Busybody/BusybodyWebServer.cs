using System;
using System.IO;
using System.Reflection;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace Busybody
{
    public class BusybodyWebServer
    {
        IDisposable _webApp;

        public void Start()
        {
            _webApp = WebApp.Start<BusybodyWebApp>(new StartOptions("http://localhost:9000"));
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
            var config = new HttpConfiguration(); 
            config.Routes.MapHttpRoute( 
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional } 
            );

            var webContentPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "webcontent");
            app.UseFileServer(new FileServerOptions()
            {
                RequestPath = PathString.Empty,
                FileSystem = new PhysicalFileSystem(webContentPath),
            });

            app.UseWebApi(config);
        }
    }
}
