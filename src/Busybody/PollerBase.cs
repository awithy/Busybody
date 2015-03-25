using System;
using System.Threading;
using System.Threading.Tasks;

namespace Busybody
{
    public abstract class PollerBase
    {
        readonly Logger _log = new Logger(typeof(PollerBase));
        readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        CancellationTokenSource _cancellationTokenSource;
        bool _stopFlag;
        bool _stopped;

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
            var task = _Poll(_cancellationTokenSource.Token);
            task.ContinueWith(t => _StartPolling(), _cancellationTokenSource.Token);

            _log.Trace("Polling thread started");
        }

        async Task _Poll(CancellationToken cancellationToken)
        {
            _log.Trace("PollerBase _Poll");
            await _OnPoll(cancellationToken);
            await Task.Delay(Period, cancellationToken);
            if (_stopFlag)
                _stoppedEvent.Set();
        }

        protected abstract Task _OnPoll(CancellationToken cancellationToken);

        protected virtual void _OnStarted()
        {
        }

        public void Stop()
        {
            _log.Debug("Stopping " + Name);
            _OnStopping();
            _stopFlag = true;
            _cancellationTokenSource.Cancel();
            var stoppedSuccessfully = _stoppedEvent.WaitOne(TimeSpan.FromMinutes(5));
            if (!stoppedSuccessfully)
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