using System;
using System.Threading;
using Busybody.Utility;

namespace Busybody
{
    public class HostTestRunnerRoleService : RoleServiceBase
    {
        readonly Logger _log = new Logger(typeof (HostTestRunnerRoleService));

        public override string Name
        {
            get { return "Host Test Runner Role Service"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromSeconds(AppContext.Instance.Config.PollingInterval); }
        }

        protected override void _OnPoll(CancellationToken cancellationToken)
        {
            _RunHostTests(cancellationToken);
        }

        void _RunHostTests(CancellationToken cancellationToken)
        {
            _log.Trace("Running host tests");
            var testRunnerCore = new HostTestRunnerCore();
            testRunnerCore.RunHostTests(cancellationToken);
        }
    }
}
