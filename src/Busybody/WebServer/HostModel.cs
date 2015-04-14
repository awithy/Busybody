namespace Busybody.WebServer
{
    public class HostModel
    {
        public string Name { get; set; }
        public string State { get; set; }
        public bool IsDanger { get; set; }
        public string LastUpdate { get; set; }
        public string LastStateChange { get; set; }
    }
}