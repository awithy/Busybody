using System;
using System.Threading;
using System.Threading.Tasks;

namespace Busybody
{
    public abstract class PollerBase
    {
        readonly Logger _log = new Logger(typeof(PollerBase));
        CancellationTokenSource _cancellationTokenSource;
        Task _task;

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

            _cancellationTokenSource = new CancellationTokenSource();
            _task = _Poll(_cancellationTokenSource.Token);
            _task.ContinueWith(t => _StartPolling(), _cancellationTokenSource.Token);

            _log.Trace("Polling thread started");
        }

        async Task _Poll(CancellationToken cancellationToken)
        {
            try
            {
                _log.Trace("PollerBase _Poll");
                var error = false;
                try
                {
                    await _OnPoll(cancellationToken);

                }
                catch (Exception ex)
                {
                    _log.ErrorFormat(ex, "Unexpected {0} exception thrown while polling in {1}", ex.GetType().Name, Name);
                    error = true;
                }

                if(error)
                    await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
                else
                    await Task.Delay(Period, cancellationToken);

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
            var result = _task.Wait(TimeSpan.FromMinutes(5));
            if (!result)
                throw new FailedWaitingForStopException(Name);
            
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