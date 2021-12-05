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
        private IDownstreamRouteProvider _finder;
        private IDownstreamRouteProvider _creator;

        public MixedDownstreamRouteProvider(IDownstreamRouteProvider finder, IDownstreamRouteProvider creator)
        {
            _finder = finder;
            _creator = creator;
        }

        ///<inheritdoc/>
        public Response<DownstreamRoute> Get(string upstreamUrlPath, string upstreamQueryString,
            string upstreamHttpMethod,
            IInternalConfiguration configuration, string upstreamHost)
        {
            
            var finderResult = _finder.Get(upstreamUrlPath, upstreamQueryString, upstreamHttpMethod, configuration,
                upstreamHost);
            return finderResult.IsError
                ? _creator.Get(upstreamUrlPath, upstreamQueryString, upstreamHttpMethod, configuration, upstreamHost)
                : finderResult;
        }
    }
}