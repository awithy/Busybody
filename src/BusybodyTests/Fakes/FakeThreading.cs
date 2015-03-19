using System.Collections.Generic;
using Busybody;

namespace BusybodyTests.Fakes
{
    public class FakeThreading : IThreading
    {
        public List<int> _sleeps = new List<int>();

        public void Sleep(int m)
        {
            _sleeps.Add(m);
        }
    }
}