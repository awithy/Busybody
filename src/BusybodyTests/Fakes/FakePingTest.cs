using System;
using System.Threading;
using Busybody;
using Busybody.Config;
using NUnit.Framework;

namespace BusybodyTests.Fakes
{
    public class FakePingTest : IBusybodyTest
    {
        public int ExecutedCount;
        int _stubIndex;
        bool[] _stubResults;
        AutoResetEvent _executionOccurredEvent = new AutoResetEvent(false);

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            ExecutedCount++;
            _executionOccurredEvent.Set();

            if (_stubResults == null)
                return true;
            bool result;
            if (_stubResults.Length < _stubIndex + 1)
                result = _stubResults[_stubResults.Length - 1];
            else
                result = _stubResults[_stubIndex++];
            return result;
        }

        public void WaitForNumberOfExecutions(int count)
        {
            while (ExecutedCount < count)
            {
                var result = _executionOccurredEvent.WaitOne(TimeSpan.FromSeconds(5));
                if (!result)
                    Assert.Fail("Failed waiting for ping test executions");
            }
        }

        public void StubResult(bool result)
        {
            _stubResults = new bool[1];
            _stubResults[0] = result;
        }

        public void StubResult(bool[] stubResults)
        {
            _stubResults = stubResults;
        }
    }
}