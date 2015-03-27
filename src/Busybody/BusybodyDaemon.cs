using Busybody.Events;

namespace Busybody
{
    public class BusybodyDaemon
    {
        readonly Logger _log = new Logger(typeof(BusybodyDaemon));
        readonly HostTestRunnerRoleService _hostTestRunnerRoleService = new HostTestRunnerRoleService();
        readonly EventProcessorRoleService _eventProcessorRoleService = new EventProcessorRoleService();

        public void Start()
        {
            _SubscribeTextEventLogger();

            AppContext.Instance.EventBus.RegisterHandler("All", typeof (AlertingEventHandler));

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

        void _SubscribeTextEventLogger()
        {
            var eventSubscription = new EventSubscription
            {
                EventStreamName = "All",
                Name = "Event Logger",
                Recipient = e => AppContext.Instance.EventLogger.Publish(e.Event.ToLogString()),
            };
            AppContext.Instance.EventBus.Subscribe(eventSubscription);
        }
    }
}