using System;
using System.Threading;
using System.Threading.Tasks;

namespace Busybody
{
    public abstract class PollerBase
    {
        readonly Logger _log = new Logger(typeof(PollerBase));
        readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public abstract string Name { get; }

        public abstract TimeSpan Period { get; }

        public void Start()
        {
            _log.Debug("Starting " + Name);
            _StartPolling();
            _OnStarted();
        }

        void _StartPolling()
        {
            _log.Trace("Starting polling thread");

            _StartPollingTaskLoop();

            _log.Trace("Polling thread started");
        }

        void _StartPollingTaskLoop()
        {
            var task = new Task(() => _Poll(_cancellationTokenSource.Token), _cancellationTokenSource.Token, TaskCreationOptions.None);
            task.ContinueWith(t => Task.Delay(Period)
                .ContinueWith(u => _StartPollingTaskLoop()));

            task.Start();
        }

        void _Poll(CancellationToken cancellationToken)
        {
            try
            {
                _log.Trace("PollerBase _Poll");
                try
                {
                    _OnPoll(cancellationToken);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat(ex, "Unexpected {0} exception thrown while polling in {1}", ex.GetType().Name, Name);
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
            _cancellationTokenSource.Cancel();
            _log.Debug(Name + " stopped");
        }

        protected virtual void _OnStopping()
        {
        }
    }
}