namespace Busybody.Events
{
    public class StartupCompleteEvent : BusybodyEvent
    {
        public override string ToLogString()
        {
            return "Startup complete";
        }
    }
}