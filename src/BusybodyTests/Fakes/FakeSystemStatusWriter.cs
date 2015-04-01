using Busybody;

namespace BusybodyTests.Fakes
{
    public class FakeSystemStatusWriter : ISystemStatusWriter
    {
        public string LastStatusText { get; set; }

        public void Write(string systemStatusText)
        {
            LastStatusText = systemStatusText;
        }
    }
}