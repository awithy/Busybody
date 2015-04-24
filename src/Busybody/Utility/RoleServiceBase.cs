using System;
using System.Threading;
using System.Threading.Tasks;

namespace Busybody.Utility
{
    public abstract class RoleServiceBase
    {
        readonly Logger _log = new Logger(typeof(RoleServiceBase));
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        Timer _timer;

        public abstract string Name { get; }

        public abstract TimeSpan Period { get; }

        public void Start()
        {
            _log.Debug("Starting " + Name);
            AppContext.Instance.SystemStatus.RoleServiceStarted(Name);
            _StartPolling();
            _OnStarted();
        }

        void _StartPolling()
        {
            _log.Trace("Starting polling thread");

            _timer = new Timer(_Callback, null, TimeSpan.Zero, Period);

            _log.Trace("Polling thread started");
        }

        void _Callback(object state)
        {
            try
            {
                _log.Trace("RoleServiceBase _Poll");
                try
                {
                    var pollStartTime = DateTime.UtcNow;

                    _OnPoll(_cancellationTokenSource.Token);

                    var pollStopTime = DateTime.UtcNow;
                    var duration = pollStopTime - pollStartTime;
                    AppContext.Instance.SystemStatus.SubmitRoleServiceStatus(Name, pollStopTime, duration);
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format("Unexpected {0} exception thrown while polling in {1}", ex.GetType().Name, Name);
                    _log.Error(errorMessage, ex);
                    _log.Debug("Cooling off from unknown error.");
                    AppContext.Instance.SystemStatus.SubmitError(Name, errorMessage);
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }
            catch (TaskCanceledException)
            {
            }
        }

        protected abstract void _OnPoll(CancellationToken cancellationToken);

        protected virtual void _OnStarted()
        {
        }

        public void Stop()
        {
            _log.Debug("Stopping " + Name);
            _OnStopping();
            _timer.Dispose();
            _cancellationTokenSource.Cancel();
            _log.Debug(Name + " stopped");
        }

        protected virtual void _OnStopping()
        {
        }
    }
}