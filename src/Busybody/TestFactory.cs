using Busybody.Config;
using Busybody.Tests;

namespace Busybody
{
    public interface ITestFactory
    {
        IBusybodyTest Create(string name);
    }

    public interface IBusybodyTest
    {
        bool Execute(HostConfig host, HostTestConfig test);
    }

    public class TestFactory : ITestFactory
    {
        static Logger _log = new Logger(typeof(TestFactory));

        public IBusybodyTest Create(string name)
        {
            _log.Trace("Creating test " + name);
            switch (name)
            {
                case "Ping":
                    var result = new PingTest();
                    return result;
                default:
                    throw new TestNotFoundException(name);
            }
        }
    }
}