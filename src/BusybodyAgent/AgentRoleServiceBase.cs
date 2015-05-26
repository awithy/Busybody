using System;
using System.Threading;
using System.Threading.Tasks;
using BusybodyShared;

namespace BusybodyAgent
{
    public abstract class AgentRoleServiceBase
    {
        readonly Logger _log = new Logger(typeof(AgentRoleServiceBase));
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        Timer _timer;

        public abstract string Name { get; }

        public abstract TimeSpan Period { get; }

        public void Start()
        {
            _log.Debug("Starting " + Name);
            _Initialize(DateTime.UtcNow);
            _StartPolling();
            _OnStarted();
        }

        protected virtual void _Initialize(DateTime time)
        {
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
                    _OnPoll(_cancellationTokenSource.Token);
                }
                catch (Exception ex)
                {
                    var errorMessage = string.Format("Unexpected {0} exception thrown while polling in {1}", ex.GetType().Name, Name);
                    _log.Error(errorMessage, ex);
                    _log.Debug("Cooling off from unknown error.");
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