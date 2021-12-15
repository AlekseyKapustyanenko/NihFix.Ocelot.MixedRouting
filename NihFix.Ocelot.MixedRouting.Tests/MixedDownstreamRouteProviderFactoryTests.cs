using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using Ocelot.Configuration;
using Ocelot.Configuration.File;
using Ocelot.DownstreamRouteFinder.Finder;
using Ocelot.Logging;
using Ocelot.Values;
using Xunit;

namespace NihFix.Ocelot.MixedRouting.Tests
{
    public class MixedDownstreamRouteProviderFactoryTests
    {
        private readonly IDownstreamRouteProvider _creatorProvider;
        private readonly IDownstreamRouteProvider _finderProvider;
        private readonly IDownstreamRouteProvider _mixedProvider;
        private readonly MixedDownstreamRouteProviderFactory _mixedProviderFactory;
        private readonly IInternalConfiguration _configuration;

        public MixedDownstreamRouteProviderFactoryTests()
        {
            _creatorProvider = Substitute.For<IDownstreamRouteProvider>();
            _finderProvider = Substitute.For<IDownstreamRouteProvider>();
            _mixedProvider = Substitute.For<IDownstreamRouteProvider>();
            _mixedProviderFactory = MixedDownstreamRouteProviderFactory.Create(
                new Dictionary<string, IDownstreamRouteProvider>
                {
                    {nameof(DownstreamRouteCreator), _creatorProvider},
                    {nameof(DownstreamRouteFinder), _finderProvider},
                    {nameof(MixedDownstreamRouteProvider), _mixedProvider}
                }, Substitute.For<IOcelotLoggerFactory>());
            _configuration = Substitute.For<IInternalConfiguration>();
        }

        [Fact]
        public async Task Get_OnlyServiceDiscoveryConfiguration_ShouldGetCreatorProvider()
        {
            _configuration.ReRoutes.Returns(new List<ReRoute>());
            _configuration.ServiceProviderConfiguration.Returns(new ServiceProviderConfiguration("ServiceDiscoveryType",
                String.Empty, 1, String.Empty, string.Empty, 0));

            var provider = _mixedProviderFactory.Get(_configuration);

            Assert.Equal(_creatorProvider, provider);
        }
        
        [Fact]
        public async Task Get_OnlyLocalConfiguration_ShouldGetFinderProvider()
        {
            _configuration.ReRoutes.Returns(new List<ReRoute>
            {
                new ReRoute(new List<DownstreamReRoute>(), new List<AggregateReRouteConfig>(), new List<HttpMethod>(),
                    new UpstreamPathTemplate("template", 1, true, "destination"), "source", "aggregator")
            });

            var provider = _mixedProviderFactory.Get(_configuration);

            Assert.Equal(_finderProvider, provider);
        }

        [Fact]
        public async Task Get_BothConfiguration_ShouldGetMixedProvider()
        {
            _configuration.ServiceProviderConfiguration.Returns(new ServiceProviderConfiguration("ServiceDiscoveryType",
                "Host", 1, String.Empty, string.Empty, 0));
            _configuration.ReRoutes.Returns(new List<ReRoute>
            {
                new ReRoute(new List<DownstreamReRoute>(), new List<AggregateReRouteConfig>(), new List<HttpMethod>(),
                    new UpstreamPathTemplate("template", 1, true, "destination"), "source", "aggregator")
            });

            var provider = _mixedProviderFactory.Get(_configuration);

            Assert.Equal(_mixedProvider, provider);
        }
        
        [Fact]
        public async Task Get_EmptyConfiguration_ShouldGetCreatorProvider()
        {
            _configuration.ReRoutes.Returns(new List<ReRoute>());
            var provider = _mixedProviderFactory.Get(_configuration);

            Assert.Equal(_creatorProvider, provider);
        }
    }
}