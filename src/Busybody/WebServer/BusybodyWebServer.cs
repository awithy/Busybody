using System;
using System.Runtime.InteropServices;
using Microsoft.Owin.Hosting;

namespace Busybody.WebServer
{
    public class BusybodyWebServer
    {
        IDisposable _webApp;

        public void Start()
        {
            try
            {
                var startOptions = new StartOptions();
                foreach(var listeningUrl in AppContext.Instance.Config.GetListeningUrls())
                    startOptions.Urls.Add(listeningUrl);
                _webApp = WebApp.Start<BusybodyWebApp>(startOptions);
            }
            catch (Exception ex)
            {
                new ErrorHandler().Critical(ex, "Exception thrown while starting web server");
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                if(_webApp != null)
                    _webApp.Dispose();
            }
            catch (Exception ex)
            {
                new ErrorHandler().Critical(ex, "Exception thrown while stopping web server");
                throw;
            }
        }
    }
}