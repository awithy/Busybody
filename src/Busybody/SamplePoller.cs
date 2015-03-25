using System;
using System.Threading;
using System.Threading.Tasks;

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

        protected override Task _OnPoll(CancellationToken cancellationToken)
        {
            return Task.Run(() => _log.Debug("Sample"), cancellationToken);
        }
    }
}
