namespace Busybody.Config
{
    public class HostTestConfig
    {
        public virtual string Name { get; set; }

        public HostTestConfig(string name)
        {
            Name = name;
        }
    }
}