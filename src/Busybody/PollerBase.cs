using System;
using System.Threading;
using System.Threading.Tasks;

namespace Busybody
{
    public abstract class PollerBase
    {
        readonly Logger _log = new Logger(typeof(PollerBase));
        CancellationTokenSource _cancellationTokenSource;

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

            //This is a bit of a mess
            _cancellationTokenSource = new CancellationTokenSource();
            var task = Task.Factory.StartNew(() => _Poll(_cancellationTokenSource.Token).Wait(),
                _cancellationTokenSource.Token,
                TaskCreationOptions.None,
                TaskScheduler.Default);
            var delayTask = task.ContinueWith(t => Task.Delay(Period),
                _cancellationTokenSource.Token,
                TaskContinuationOptions.NotOnFaulted,
                TaskScheduler.Default);
            delayTask.ContinueWith(t =>
            {
                t.Wait();
                _StartPolling();
            },
            _cancellationTokenSource.Token,
            TaskContinuationOptions.NotOnFaulted,
            TaskScheduler.Default);

            _log.Trace("Polling thread started");
        }

        async Task _Poll(CancellationToken cancellationToken)
        {
            try
            {
                _log.Trace("PollerBase _Poll");
                try
                {
                    await _OnPoll(cancellationToken);
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

        protected abstract Task _OnPoll(CancellationToken cancellationToken);

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

    public class FailedWaitingForStopException : Exception
    {
        public FailedWaitingForStopException(string name) : base(string.Format("Role service {0} failed waiting to stop.", name))
        {
        }
    }
}