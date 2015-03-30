namespace Busybody.Events
{
    public class SystemErrorEvent : BusybodyEvent
    {
        public string Message { get; set; }
        public string Detail { get; set; }

        public SystemErrorEvent(string message, string detail)
        {
            Message = message;
            Detail = detail;
        }
    }
}