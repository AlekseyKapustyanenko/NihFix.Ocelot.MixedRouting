using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Finder;

namespace NihFix.Ocelot.MixedRouting
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add ability to combine dynamic routing(with service discovery)
        /// and routing from configuration file.
        /// </summary>
        /// <param name="ocelotBuilder">Ocelot builder.</param>
        /// <returns>Ocelot builder.</returns>
        public static IOcelotBuilder AddMixedRouting(this IOcelotBuilder ocelotBuilder)
        {
            ocelotBuilder.Services
                .RemoveAll(typeof(IDownstreamRouteProviderFactory))
                .AddSingleton<IDownstreamRouteProviderFactory, MixedDownstreamRouteProviderFactory>()
                .AddSingleton<IDownstreamRouteProvider, MixedDownstreamRouteProvider>();
            return ocelotBuilder;
        }
    }
}