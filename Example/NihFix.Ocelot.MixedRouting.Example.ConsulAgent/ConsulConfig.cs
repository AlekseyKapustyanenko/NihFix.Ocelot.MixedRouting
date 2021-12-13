namespace NihFix.Ocelot.MixedRouting.Example.ConsulAgent
{
    /// <summary>
    /// Config for Consul ServiceDiscovery.
    /// </summary>
    public class ConsulConfig
    {
        public static string Consul => "Consul";
        
        /// <summary>
        /// Consul host and port.
        /// </summary>
        public string Address { get; set; }
        
        /// <summary>
        /// Service name for registration in Consul.
        /// </summary>
        public string ServiceName { get; set; }
    }
}