namespace Busybody.Config
{
    public class HostTestConfig
    {
        public virtual string Name { get; set; }
        public virtual string HostNickname { get; set; }

        public HostTestConfig(string name)
        {
            Name = name;
        }
    }
}