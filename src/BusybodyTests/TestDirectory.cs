using System;
using System.Diagnostics;
using System.IO;
using Busybody;

namespace BusybodyTests
{
    public class TestDirectory : IDisposable
    {
        public string RootPath { get; private set; }

        public TestDirectory()
        {
            RootPath = Path.Combine(Path.GetTempPath(), "Busybody", CommonPaths.RandomName());
            Directory.CreateDirectory(RootPath);
            Debug.Write(RootPath);
        }

        public void Dispose()
        {
            try
            {
                //For now I'd prefer to see what's up with the test directories in event of test failure.  Warning: Disk space could be an issue.
                //Directory.Delete(RootPath, true);
            }
            catch // Intentional
            {
            }
        }

        public string FilePathFor(string configFileName)
        {
            return Path.Combine(RootPath, configFileName);
        }
    }
}