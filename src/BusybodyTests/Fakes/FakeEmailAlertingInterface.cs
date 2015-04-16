using System;
using System.Collections.Generic;
using System.Threading;
using Busybody;
using Busybody.Utility;

namespace BusybodyTests.Fakes
{
    public class FakeEmailAlertingInterface : IEmailAlertingInterface
    {
        public List<EmailAlert> EmailAlerts = new List<EmailAlert>();
        readonly AutoResetEvent _emailEvent = new AutoResetEvent(false);

        public void Alert(EmailAlert emailAlert)
        {
            EmailAlerts.Add(emailAlert);
            _emailEvent.Set();
        }

        public bool WaitForEmails(int numberOfEmails)
        {
            while (EmailAlerts.Count < numberOfEmails)
            {
                var result = _emailEvent.WaitOne(TimeSpan.FromSeconds(5));
                if (!result)
                    return false;
            }
            return true;
        }
    }
}