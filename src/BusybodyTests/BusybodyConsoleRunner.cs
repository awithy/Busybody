using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace BusybodyTests
{
    public class BusybodyConsoleRunner : IDisposable
    {
        Process _process;
        ProcessStartInfo _processStartInfo;

        public BusybodyConsoleRunner(string workingDirectory)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var consoleExePath = Path.Combine(assemblyDirectory, "Busybody.exe");
            _processStartInfo = new ProcessStartInfo(consoleExePath)
            {
                WorkingDirectory = workingDirectory,
            };
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

        public void Start()
        {
            _process = Process.Start(_processStartInfo);
        }
    }
}