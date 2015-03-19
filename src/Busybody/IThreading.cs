using System.Threading;

namespace Busybody
{
    public interface IThreading
    {
        void Sleep(int m);
    }

    public class Threading : IThreading
    {
        public void Sleep(int m)
        {
            Thread.Sleep(m);
        }
    }

}