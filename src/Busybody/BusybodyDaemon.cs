﻿using Busybody.Events;

namespace Busybody
{
    public class BusybodyDaemon
    {
        readonly Logger _log = new Logger(typeof(BusybodyDaemon));
        readonly HostTestRunnerRoleService _hostTestRunnerRoleService = new HostTestRunnerRoleService();
        readonly EventProcessorRoleService _eventProcessorRoleService = new EventProcessorRoleService();

        public void Start()
        {
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (AlertingEventHandler));
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (HostEventHandler));
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (EventLogger));

            _hostTestRunnerRoleService.Start();
            _eventProcessorRoleService.Start();

            AppContext.Instance.EventBus.Publish("All", new StartupCompleteEvent());
            _log.Info("Busybody started");
        }

        public void Stop()
        {
            _log.Info("Stopping");

            _hostTestRunnerRoleService.Stop();
            _eventProcessorRoleService.Stop();

            _log.Info("Stopped");
        }
    }
}