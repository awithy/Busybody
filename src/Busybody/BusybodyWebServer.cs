using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public class HostModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string LastUpdate { get; set; }
        public string LastStateChange { get; set; }
    }

    public class HostsController : ApiController
    {
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
}