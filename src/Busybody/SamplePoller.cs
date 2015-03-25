using System;

namespace Busybody
{
    public class SamplePoller : PollerBase
    {
        readonly Logger _log = new Logger(typeof(SamplePoller));

        public override string Name
        {
            get { return "Sample"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromSeconds(5); }
        }

        protected override void _OnPoll()
        {
            _log.Info("Sample");
        }
    }
}
