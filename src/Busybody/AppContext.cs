using System.Collections;
using Busybody.Config;

namespace Busybody
{
    public class AppContext : IAppContext
    {
        public static IAppContext Instance;

        public BusybodyConfig Config { get; set; }
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }

        public AppContext()
        {
            EventLogger = new EventLogger();
            TestFactory = new TestFactory();
        }
    }

    public interface IAppContext
    {
        IEventLogger EventLogger { get; }
        ITestFactory TestFactory { get; }
        BusybodyConfig Config { get; set; }
    }
}