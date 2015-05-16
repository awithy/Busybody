using System;
using System.Threading;

namespace BusybodyAgent
{
    public class BusybodyAgentDaemon
    {
        AgentCoreRoleService _agentCoreRoleService;

        public void Start()
        {
            _agentCoreRoleService = new AgentCoreRoleService();
            _agentCoreRoleService.Start();
        }

        public void Stop()
        {
            _agentCoreRoleService.Stop();
        }
    }

    public class AgentCoreRoleService : AgentRoleServiceBase
    {
        AgentCore _agentCore;

        public override string Name
        {
            get { return "Agent Core"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromSeconds(AppContext.Instance.Config.PollingInterval); }
        }

        protected override void _Initialize(DateTime time)
        {
            _agentCore = new AgentCore();
            base._Initialize(time);
        }

        protected override void _OnPoll(CancellationToken cancellationToken)
        {
            _agentCore.Poll(DateTime.UtcNow);
        }
    }
}