using Busybody.Config;

namespace Busybody
{
    public class AppContext : IAppContext
    {
        public static IAppContext Instance;

        public BusybodyConfig Config { get; set; }
        public IEventLogger EventLogger { get; private set; }

        public AppContext()
        {
            EventLogger = new EventLogger();
        }
    }

    public interface IAppContext
    {
        IEventLogger EventLogger { get; }
        BusybodyConfig Config { get; set; }
    }
}