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
            _logger = factory.CreateLogger<DownstreamRouteProviderFactory>();
            _providers = provider.GetServices<IDownstreamRouteProvider>()
                .ToDictionary(
                    x => x.GetType().Name);
        }

        ///<inheritdoc/>
        public IDownstreamRouteProvider Get(IInternalConfiguration config)
        {
            var hasRoutesInConfig = config.ReRoutes.Any() && !config.ReRoutes.All(
                x => string.IsNullOrEmpty(x.UpstreamTemplatePattern?.OriginalValue));
            var isServiceDiscovery = IsServiceDiscovery(config.ServiceProviderConfiguration);
            if (hasRoutesInConfig && isServiceDiscovery)
            {
                _logger.LogInformation(
                    "Selected MixedDownstreamRouteProvider as DownstreamRouteProvider for this request");
                return _providers[nameof(MixedDownstreamRouteProvider)];
            }
            else if (isServiceDiscovery && !hasRoutesInConfig)
            {
                _logger.LogInformation("Selected DownstreamRouteCreator as DownstreamRouteProvider for this request");
                return _providers[nameof(DownstreamRouteCreator)];
            }

            _logger.LogInformation("Selected DownstreamRouteFinder as DownstreamRouteProvider for this request");

            return _providers[nameof(DownstreamRouteFinder)];
        }

        private bool IsServiceDiscovery(ServiceProviderConfiguration config) => !string.IsNullOrEmpty(config?.Host) &&
            config != null && (config.Port > 0 && !string.IsNullOrEmpty(config.Type));
    }
}