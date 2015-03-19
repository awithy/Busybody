using System.Collections.Generic;
using Busybody;

namespace BusybodyTests.Fakes
{
    public class FakeTestFactory : ITestFactory
    {
        public Dictionary<string, IBusybodyTest> Tests = new Dictionary<string, IBusybodyTest>();

        public IBusybodyTest Create(string name)
        {
            return Tests[name];
        }
    }
}