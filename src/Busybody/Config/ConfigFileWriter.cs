using System;
using System.IO;
using System.Text;

namespace Busybody.Config
{
    public class ConfigFileWriter
    {
        public void Write(BusybodyConfig busybodyConfig, string filename)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#Busybody config");
            foreach (var host in busybodyConfig.Hosts)
            {
                sb.Append("Host,");
                sb.Append(host.Nickname);
                sb.Append(",");
                sb.Append(host.Hostname);
                sb.Append(Environment.NewLine);

                foreach (var test in host.Tests)
                {
                    sb.Append("HostTest,");
                    sb.Append(test.HostNickname);
                    sb.Append(",");
                    sb.Append(test.Name);
                    sb.Append(Environment.NewLine);
                }
            }

            File.WriteAllText(filename, sb.ToString());
        }
    }
}