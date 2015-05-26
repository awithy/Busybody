using System.Collections.Generic;

namespace Busybody.WebServer
{
    public class HostsByGroupModel
    {
        public IEnumerable<HostGroupModel> HostGroups { get; set; }

        public HostsByGroupModel()
        {
            HostGroups = new HostGroupModel[0];
        }
    }

    public class HostGroupModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public IEnumerable<HostModel> Hosts { get; set; }

        public HostGroupModel()
        {
            Hosts = new HostModel[0];
        }
    }

    public class HostModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Hostname { get; set; }
        public string State { get; set; }
        public string LastUpdate { get; set; }
        public string LastStateChange { get; set; }
        public string Location { get; set; }
        public string Group { get; set; }
        public IEnumerable<HostTestModel> Tests { get; set; }

        public HostModel()
        {
            Tests = new HostTestModel[0];
        }
    }

    public class HostTestModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public string LastTest { get; set; }
    }
}