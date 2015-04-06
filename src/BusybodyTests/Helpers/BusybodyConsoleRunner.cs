using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace BusybodyTests.Helpers
{
    public class BusybodyConsoleRunner : IDisposable
    {
        Process _process;
        ProcessStartInfo _processStartInfo;

        public BusybodyConsoleRunner(string workingDirectory, string configFilePath = null)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists("Busybody.exe.config"))
                File.Copy("BusybodyTests.dll.config", "Busybody.exe.config");
            var consoleExePath = Path.Combine(assemblyDirectory, "Busybody.exe");
            _processStartInfo = new ProcessStartInfo(consoleExePath) { WorkingDirectory = workingDirectory, };
            if (configFilePath != null)
                _processStartInfo.Arguments = string.Format("-c:{0}", configFilePath);
        }

        public void Dispose()
        {
            try
            {
                _process.Kill();
                Thread.Sleep(500); // Done because the process was holding onto one of the log files in the console tests resulting in an IOException.  There's probably a better way of doing this.
            }
            catch // Intentional
            {
            }
        }

        public void Start()
        {
            _process = Process.Start(_processStartInfo);
        }
    }
}