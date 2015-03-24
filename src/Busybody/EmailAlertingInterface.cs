namespace Busybody
{
    public interface IEmailAlertingInterface
    {
        void Alert(EmailAlert emailAlert);
    }

    public class EmailAlertingInterface : IEmailAlertingInterface
    {
        public void Alert(EmailAlert emailAlert)
        {
            
        }
    }

    public class EmailAlert
    {
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}