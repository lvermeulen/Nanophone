namespace SampleService.AspNetCore.Kestrel
{
    public class AppSettings
    {
        public Consul Consul { get; set; }
    }

    public class Consul
    {
        public string HostName { get; set; }
        public int? Port { get; set; }
    }
}
