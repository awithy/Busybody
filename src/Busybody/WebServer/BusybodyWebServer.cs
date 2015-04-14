using System;
using Microsoft.Owin.Hosting;

namespace Busybody.WebServer
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
}