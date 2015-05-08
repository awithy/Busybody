namespace Busybody.Events
{
    public class EmailAlertSentEvent : BusybodyEvent
    {
        public string Recipients { get; private set; }
        public string Subject { get; private set; }

        public EmailAlertSentEvent(string recipients, string subject)
        {
            Recipients = recipients;
            Subject = subject;
        }

        public override string ToLogString()
        {
            return string.Format("E-mail alert '{0}' sent to '{1}'", Subject, Recipients);
        }
    }
}