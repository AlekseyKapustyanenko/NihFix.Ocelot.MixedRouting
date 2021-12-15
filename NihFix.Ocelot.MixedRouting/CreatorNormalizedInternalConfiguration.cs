using System.Collections.Generic;
using System.Linq;
using Ocelot.Configuration;

namespace NihFix.Ocelot.MixedRouting
{
    /// <summary>
    /// Internal configuration which is applicable for DownstreamRouteCreator. 
    /// </summary>
    public class CreatorNormalizedInternalConfiguration: IInternalConfiguration
    {
        public CreatorNormalizedInternalConfiguration(IInternalConfiguration internalConfiguration)
        {
            ReRoutes = ExcludeStandardReRotes(internalConfiguration.ReRoutes);
            AdministrationPath = internalConfiguration.AdministrationPath;
            ServiceProviderConfiguration = internalConfiguration.ServiceProviderConfiguration;
            RequestId = internalConfiguration.RequestId;
            LoadBalancerOptions = internalConfiguration.LoadBalancerOptions;
            DownstreamScheme = internalConfiguration.DownstreamScheme;
            QoSOptions = internalConfiguration.QoSOptions;
            HttpHandlerOptions = internalConfiguration.HttpHandlerOptions;
        }
        
        public List<ReRoute> ReRoutes { get; }
        
        public string AdministrationPath { get; }
        
        public ServiceProviderConfiguration ServiceProviderConfiguration { get; }
        
        public string RequestId { get; }
        
        public LoadBalancerOptions LoadBalancerOptions { get; }
        
        public string DownstreamScheme { get; }
        
        public QoSOptions QoSOptions { get; }
        
        public HttpHandlerOptions HttpHandlerOptions { get; }

        private List<ReRoute> ExcludeStandardReRotes(List<ReRoute> routes)
        {
            return routes?.Where(r => r.UpstreamTemplatePattern == null).ToList();
        }
    }
}