namespace Busybody.Events
{
    public class HostStateEvent : BusybodyEvent
    {
        public string HostNickname { get; set; }
        public string StateText { get; set; }
        public HostState State { get; set; }

        public HostStateEvent(string hostNickname, HostState hostState)
        {
            HostNickname = hostNickname;
            StateText = "Host: " + hostNickname + ", State: " + hostState;
            State = hostState;
        }

        public override string ToLogString()
        {
            return StateText;
        }
    }

    public enum HostState
    {
        UNKNOWN = 0,
        UP = 1,
        DOWN = 2,
    }
}