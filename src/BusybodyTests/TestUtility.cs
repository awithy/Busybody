using System.IO;
using System.Threading;

namespace BusybodyTests
{
    public class TestUtility
    {
        public static void DeleteDirectoryWithRetries(string directoryPath)
        {
            var tries = 0;
            while (true)
            {
                try
                {
                    if (Directory.Exists(directoryPath))
                        Directory.Delete(directoryPath, true);
                    Directory.CreateDirectory(directoryPath);
                    return;
                }
                catch (IOException)
                {
                    if(tries > 10)
                        throw;
                    Thread.Sleep(100);
                }
            }
        }
    }
}