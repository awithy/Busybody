using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Busybody
{
    public class SystemMonitorRoleService : RoleServiceBase
    {
        DateTime _systemUptime = DateTime.UtcNow;
        Logger _log = new Logger(typeof(SystemMonitorRoleService));

        public override string Name
        {
            get { return "System Monitor"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromMinutes(1); }
        }

        protected override void _OnPoll(CancellationToken cancellationToken)
        {
            _log.Trace("Writing system status");
            var roleServiceStatuses = AppContext.Instance.SystemMonitorData.GetRoleServiceHealthStatus();
            var sb = new StringBuilder();
            sb.AppendLine("# Busybody System Status #");
            sb.AppendLine();
            sb.AppendLine("Started: " + _systemUptime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine();
            sb.AppendLine("Total Role Services: " + roleServiceStatuses.Count());
            sb.AppendLine("Healthy Role Services: " + roleServiceStatuses.Count(x => x.RoleServiceHealth == RoleServiceHealth.Healthy));
            sb.AppendLine("Errored Role Services: " + roleServiceStatuses.Count(x => x.RoleServiceHealth == RoleServiceHealth.Error));
            sb.AppendLine();
            sb.AppendLine("## Role Services ##");
            sb.AppendLine();
            foreach (var roleServiceStatus in roleServiceStatuses)
            {
                sb.AppendLine("Role Service: **" + roleServiceStatus.Name + "**");
                sb.AppendLine("Status: " + roleServiceStatus.RoleServiceHealth);
                sb.AppendLine("Last Poll: " + roleServiceStatus.LastPoll);
                sb.AppendLine("Last Poll Duration: " + roleServiceStatus.LastPollDuration);
                if (roleServiceStatus.RoleServiceHealth == RoleServiceHealth.Error)
                    sb.AppendLine("Last Error Message: " + roleServiceStatus.LastErrorMessage);
                sb.AppendLine();
            }
            AppContext.Instance.SystemStatusWriter.Write(sb.ToString());
        }
    }

    public class SystemMonitorData
    {
        readonly ConcurrentDictionary<string, RoleServiceHealthStatus> _roleServiceHealth = new ConcurrentDictionary<string, RoleServiceHealthStatus>();

        public void RoleServiceStarted(string name)
        {
            _roleServiceHealth.AddOrUpdate(name, n => RoleServiceHealthStatus.Started(name), (n, s) => s);
        }
        
        public void SubmitRoleServiceStatus(string name, DateTime lastPoll, TimeSpan lastPollDuration)
        {
            _roleServiceHealth.AddOrUpdate(name, n => RoleServiceHealthStatus.Started(name), (n, s) => s.UpdateStatus(lastPoll, lastPollDuration));
        }

        public void SubmitError(string name, string errorMessage)
        {
            _roleServiceHealth.AddOrUpdate(name, n => RoleServiceHealthStatus.Started(name), (n, s) => s.Error(errorMessage));
        }

        public IEnumerable<RoleServiceHealthStatus> GetRoleServiceHealthStatus()
        {
            return _roleServiceHealth.Values.ToArray();
        }
    }

    public class RoleServiceHealthStatus
    {
        public string Name { get; set; }
        public string LastErrorMessage { get; set; }
        public DateTime LastPoll { get; set; }
        public TimeSpan LastPollDuration { get; set; }
        public RoleServiceHealth RoleServiceHealth { get; set; }

        public static RoleServiceHealthStatus Started(string name)
        {
            return new RoleServiceHealthStatus
            {
                Name = name,
                LastErrorMessage = null,
                LastPoll = DateTime.MinValue,
                LastPollDuration = TimeSpan.Zero,
                RoleServiceHealth = RoleServiceHealth.Healthy,
            };
        }

        public RoleServiceHealthStatus UpdateStatus(DateTime lastPoll, TimeSpan lastPollDuration)
        {
            return new RoleServiceHealthStatus
            {
                Name = Name,
                LastPoll = lastPoll,
                LastPollDuration = lastPollDuration,
                RoleServiceHealth = RoleServiceHealth.Healthy,
                LastErrorMessage = null,
            };
        }

        public RoleServiceHealthStatus Error(string errorMessage)
        {
            return new RoleServiceHealthStatus
            {
                Name = Name,
                LastPoll = LastPoll,
                LastPollDuration = LastPollDuration,
                RoleServiceHealth = RoleServiceHealth.Error,
                LastErrorMessage = errorMessage,
            };
        }
    }

    public enum RoleServiceHealth
    {
        Unknown,
        Healthy,
        Error,
    }

    public interface ISystemStatusWriter
    {
        void Write(string systemStatusText);
    }

    public class SystemStatusWriter : ISystemStatusWriter
    {
        public void Write(string systemStatusText)
        {
            var filePath = Path.Combine(CommonPaths.BusybodyData(), "SystemStatus.txt");
            File.WriteAllText(filePath, systemStatusText);
        }
    }
}