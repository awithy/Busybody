namespace Busybody
{
    public class HostStateEvent : BusybodyEvent
    {
        public string StateText { get; set; }

        public HostStateEvent(string stateText)
        {
            StateText = stateText;
        }
    }
}