using Busybody;
using Busybody.Config;

namespace BusybodyTests.Fakes
{
    public class FakePingTest : IBusybodyTest
    {
        public int ExecutedCount;
        int _stubIndex;
        bool[] _stubResults;

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            ExecutedCount++;

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
            TestUtility.WaitFor(() => ExecutedCount >= count);
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