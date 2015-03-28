namespace Busybody
{
    public class HostTestResultEvent : BusybodyEvent
    {
        public string TestName { get; set; }
        public string HostNickname { get; set; }
        public bool TestResult { get; set; }
    }
}