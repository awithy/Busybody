using System.Collections.Generic;

namespace Busybody.Config
{
    public class EmailAlertConfiguration
    {
        public string FromAddress { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string ToEmailAddress { get; set; }
        public int Port { get; set; }
    }
}