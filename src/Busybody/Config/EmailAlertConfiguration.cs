using System.Collections.Generic;

namespace Busybody.Config
{
    public class EmailAlertConfiguration
    {
        public bool Enabled { get; set; }
        public string FromAddress { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string ToEmailAddress { get; set; }
        public int Port { get; set; }
    }
}