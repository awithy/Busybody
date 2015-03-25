using System;
using System.Threading;

namespace Busybody
{
    public abstract class PollerBase
    {
        readonly Logger _log = new Logger(typeof(PollerBase));
        readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        Timer _timer;
        bool _stopFlag;
        bool _stopped;

        public abstract string Name { get; }

        public abstract TimeSpan Period { get; }

        public void Start()
        {
            _StartPolling();
            _OnStarted();
        }

        void _StartPolling()
        {
            _log.Trace("Starting polling thread");
            var threadStart = new ThreadStart(() => _StartPollingThreadStart(null));
            var thread = new Thread(threadStart)
            {
                IsBackground = true
            };
            thread.Start();
            _log.Trace("Polling thread started");
        }

        void _StartPollingThreadStart(object state)
        {
            _log.Trace("Role service polling control");
            if(_timer != null)
                _timer.Dispose();
            if (_stopped)
                return;

            try
            {
                _OnPoll();

                if (!_stopFlag)
                    _timer = new Timer(_StartPollingThreadStart, null, Period, TimeSpan.FromMilliseconds(Timeout.Infinite));
                else
                    _stoppedEvent.Set();
            }
            catch (Exception ex)
            {
                _log.Error("Unexpected exception while executing polling mehtod.", ex);
                Thread.Sleep(TimeSpan.FromMinutes(5));
            }
        }

        protected virtual void _OnStarted()
        {
        }

        public void Stop()
        {
            _OnStopping();
            _stopFlag = true;
            if (!_stoppedEvent.WaitOne(TimeSpan.FromMinutes(5)))
                throw new FailedWaitingForStopException(Name);
        }

        protected virtual void _OnStopping()
        {
            
        }

        protected abstract void _OnPoll();
    }

    public class FailedWaitingForStopException : Exception
    {
        public FailedWaitingForStopException(string name) : base(string.Format("Role service {0} failed waiting to stop.", name))
        {
        }
    }
}