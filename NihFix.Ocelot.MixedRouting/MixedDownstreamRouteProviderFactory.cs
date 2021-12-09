using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.DownstreamRouteFinder.Finder;
using Ocelot.Logging;

namespace NihFix.Ocelot.MixedRouting
{
    public class MixedDownstreamRouteProviderFactory : IDownstreamRouteProviderFactory
    {
        private readonly Dictionary<string, IDownstreamRouteProvider> _providers;
        private readonly IOcelotLogger _logger;

        public MixedDownstreamRouteProviderFactory(IServiceProvider provider, IOcelotLoggerFactory factory)
        {
            _logger = factory.CreateLogger<MixedDownstreamRouteProviderFactory>();
            _providers = provider.GetServices<IDownstreamRouteProvider>()
                .ToDictionary(
                    x => x.GetType().Name);
        }

        private MixedDownstreamRouteProviderFactory(Dictionary<string, IDownstreamRouteProvider> providers,
            IOcelotLoggerFactory factory)
        {
            _logger = factory.CreateLogger<MixedDownstreamRouteProviderFactory>();
            _providers = providers;
        }

        ///<inheritdoc/>
        public IDownstreamRouteProvider Get(IInternalConfiguration config)
        {
            var hasRoutesInConfig = config.ReRoutes.Any() && !config.ReRoutes.All(
                x => string.IsNullOrEmpty(x.UpstreamTemplatePattern?.OriginalValue));
            var isServiceDiscovery = IsServiceDiscovery(config.ServiceProviderConfiguration);
            if (hasRoutesInConfig && !isServiceDiscovery)
            {
                _logger.LogInformation("Selected DownstreamRouteCreator as DownstreamRouteProvider for this request");
                return _providers[nameof(DownstreamRouteFinder)];
            }else
            if (isServiceDiscovery && !hasRoutesInConfig)
            {
                _logger.LogInformation("Selected DownstreamRouteCreator as DownstreamRouteProvider for this request");
                return _providers[nameof(DownstreamRouteCreator)];
            }
           
            else  if (hasRoutesInConfig && isServiceDiscovery)
            {
                _logger.LogInformation(
                    "Selected MixedDownstreamRouteProvider as DownstreamRouteProvider for this request");
                return _providers[nameof(MixedDownstreamRouteProvider)];
            }

            _logger.LogInformation("Selected DownstreamRouteCreator as DownstreamRouteProvider for this request");

            return _providers[nameof(DownstreamRouteCreator)];
        }

        private bool IsServiceDiscovery(ServiceProviderConfiguration config) => config != null 
            && !string.IsNullOrEmpty(config.Host) 
            && (config.Port > 0 && !string.IsNullOrEmpty(config.Type));

        public static MixedDownstreamRouteProviderFactory Create(Dictionary<string, IDownstreamRouteProvider> providers,
            IOcelotLoggerFactory factory)
        {
            return new MixedDownstreamRouteProviderFactory(providers, factory);
        }
    }
}