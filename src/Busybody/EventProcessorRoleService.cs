using System;
using System.Threading;
using System.Threading.Tasks;

namespace Busybody
{
    public class EventProcessorRoleService : PollerBase
    {
        public override string Name
        {
            get { return "Event Processor Role Service"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromMilliseconds(1000); }
        }

        protected override void _OnPoll(CancellationToken cancellationToken)
        {
            AppContext.Instance.EventBus.DispatchPending(cancellationToken);
        }
    }
}
