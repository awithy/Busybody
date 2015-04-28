namespace Busybody.WebServer
{
    public class HostModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Hostname { get; set; }
        public string State { get; set; }
        public bool IsDanger { get; set; }
        public string LastUpdate { get; set; }
        public string LastStateChange { get; set; }
        public string Location { get; set; }
    }
}