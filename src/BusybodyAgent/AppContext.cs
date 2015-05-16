using System;

namespace BusybodyAgent
{
    public interface IAppContext
    {
        DateTime StartTime { get; }
        BusybodyConfig Config { get; set;  }
    }

    public class AppContext : IAppContext
    {
        public DateTime StartTime { get; private set; }
        public static IAppContext Instance;
        public BusybodyConfig Config { get; set; }

        public AppContext()
        {
            StartTime = DateTime.UtcNow;
        }
    }
}