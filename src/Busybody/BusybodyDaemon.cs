using System;
using System.Collections.Generic;
using Busybody.Events;
using Busybody.Utility;
using Busybody.WebServer;
using BusybodyShared;

namespace Busybody
{
    public class BusybodyDaemon
    {
        readonly Logger _log = new Logger(typeof(BusybodyDaemon));
        readonly BusybodyWebServer _busybodyWebServer = new BusybodyWebServer();
        List<IRoleService> _roleServices;

        public void Start()
        {
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (AlertingEventHandler), InstanceMode.Singleton);
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (HostEventHandler));
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (EventLogger));
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (EventLogRepository));

            _roleServices = new List<IRoleService>
            {
                new HostTestRunnerRoleService(),
                new EventProcessorRoleService(),
                new SystemMonitorRoleService(),
                new AzureStatusRoleService(),
            };

            foreach (var roleService in _roleServices)
                roleService.Start();

            _busybodyWebServer.Start();

            AppContext.Instance.EventBus.Publish("All", new BusybodyStartedEvent(DateTime.UtcNow));
            _log.Info("Busybody started");
        }

        public void Stop()
        {
            _log.Info("Stopping");

            _busybodyWebServer.Stop();

            foreach (var roleService in _roleServices)
                roleService.Stop();

            _log.Info("Stopped");
        }
    }
}