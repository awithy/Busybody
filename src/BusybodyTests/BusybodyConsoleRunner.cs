using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BusybodyTests
{
    public class BusybodyConsoleRunner : IDisposable
    {
        readonly Process _process;

        public BusybodyConsoleRunner(string workingDirectory)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var consoleExePath = Path.Combine(assemblyDirectory, "Busybody.exe");
            var processStartInfo = new ProcessStartInfo(consoleExePath)
            {
                WorkingDirectory = workingDirectory,
            };
            _process = Process.Start(processStartInfo);
        }

        public void Dispose()
        {
            try
            {
                _process.Kill();
            }
            catch // Intentional
            {
            }
        }
    }
}