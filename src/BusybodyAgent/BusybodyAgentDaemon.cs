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
}