using Busybody.Events;

namespace Busybody
{
    public class BusybodyDaemon
    {
        readonly Logger _log = new Logger(typeof(BusybodyDaemon));
        readonly HostTestRunnerRoleService _hostTestRunnerRoleService = new HostTestRunnerRoleService();
        readonly EventProcessorRoleService _eventProcessorRoleService = new EventProcessorRoleService();
        readonly SystemMonitorRoleService _systemMonitorRoleService = new SystemMonitorRoleService();

        public void Start()
        {
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (AlertingEventHandler), InstanceMode.Singleton);
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (HostEventHandler));
            AppContext.Instance.EventBus.RegisterHandler("All", typeof (EventLogger));

            _systemMonitorRoleService.Start();
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
            _systemMonitorRoleService.Stop();

            _log.Info("Stopped");
        }
    }
}