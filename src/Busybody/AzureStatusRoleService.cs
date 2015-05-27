using System;
using System.Threading;
using Busybody.Utility;

namespace Busybody
{
    public class AzureStatusRoleService : RoleServiceBase
    {
        public override string Name
        {
            get { return "Azure Status"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        protected override void _OnPoll(CancellationToken cancellationToken)
        {
            var azureStatusWriter = new AzureStatusWriter();
            azureStatusWriter.Write();
        }
    }
}
