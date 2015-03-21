namespace Busybody
{
    public class StartupCompleteEvent : BusybodyEvent
    {
        public override string ToLogString()
        {
            return "Startup complete";
        }
    }
}