using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Configuration;
using Ocelot.DownstreamRouteFinder;
using Ocelot.DownstreamRouteFinder.Finder;
using Ocelot.Responses;

namespace NihFix.Ocelot.MixedRouting
{
    public class MixedDownstreamRouteProvider : IDownstreamRouteProvider
    {
        private readonly Lazy<Dictionary<string, IDownstreamRouteProvider>> _providers;

        public MixedDownstreamRouteProvider(IServiceProvider provider)
        {
            _providers = new Lazy<Dictionary<string, IDownstreamRouteProvider>>(() => provider
                .GetServices<IDownstreamRouteProvider>()
                .ToDictionary(
                    x => x.GetType().Name));
        }

        ///<inheritdoc/>
        public Response<DownstreamRoute> Get(string upstreamUrlPath, string upstreamQueryString,
            string upstreamHttpMethod,
            IInternalConfiguration configuration, string upstreamHost)
        {
            var finder = _providers.Value[nameof(DownstreamRouteFinder)];
            var creator = _providers.Value[nameof(DownstreamRouteCreator)];
            var finderResult = finder.Get(upstreamUrlPath, upstreamQueryString, upstreamHttpMethod, configuration,
                upstreamHost);
            return finderResult.IsError
                ? creator.Get(upstreamUrlPath, upstreamQueryString, upstreamHttpMethod, configuration, upstreamHost)
                : finderResult;
        }
    }
}