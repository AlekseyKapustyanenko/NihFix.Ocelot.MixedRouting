using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Ocelot.Configuration;
using Ocelot.Configuration.File;
using Ocelot.Values;
using Xunit;

namespace NihFix.Ocelot.MixedRouting.Tests
{
    public class FinderNormalizedInternalConfigurationTests
    {
        private readonly ReRoute _standardReRoute;
        private readonly ReRoute _dynamicReRoute;
        private readonly InternalConfiguration _sourceInternalConfiguration;

        public FinderNormalizedInternalConfigurationTests()
        {
            _standardReRoute = new ReRoute(
                new List<DownstreamReRoute>(),
                new List<AggregateReRouteConfig>(),
                new List<HttpMethod>(),
                new UpstreamPathTemplate("template", 0, true, "originalValue"),
                "UpstreamHost",
                "UpstreamAggregator");
            _dynamicReRoute = new ReRoute(
                new List<DownstreamReRoute>(),
                new List<AggregateReRouteConfig>(),
                null,
                null,
                "UpstreamHost",
                "UpstreamAggregator");
            _sourceInternalConfiguration = new InternalConfiguration(
                new List<ReRoute> {_standardReRoute, _dynamicReRoute},
                "administrationPath",
                new ServiceProviderConfiguration("type", "scheme",  5000, "token", "configurationKey", 1),
                "requestId",
                new LoadBalancerOptions("type", "key", 1),
                "downstramScheme",
                new QoSOptions(0, 0, 0, "key"),
                new HttpHandlerOptions(true, true, true, true)
            );
        }

        [Fact]
        public async Task Ctor_ReroutesMustBeFilteredFromDynamicRoutes_And_OtherPropsShouldBeTheSame()
        {
            var normalizedConfig = new FinderNormalizedInternalConfiguration(_sourceInternalConfiguration);

            Assert.Single(normalizedConfig.ReRoutes, _standardReRoute);
            Assert.Same(
                _sourceInternalConfiguration.AdministrationPath,
                _sourceInternalConfiguration.AdministrationPath);
            Assert.Same(
                _sourceInternalConfiguration.DownstreamScheme,
                _sourceInternalConfiguration.DownstreamScheme);
            Assert.Same(
                _sourceInternalConfiguration.RequestId,
                _sourceInternalConfiguration.RequestId);
            Assert.Same(
                _sourceInternalConfiguration.ReRoutes,
                _sourceInternalConfiguration.ReRoutes);
            Assert.Same(
                _sourceInternalConfiguration.HttpHandlerOptions,
                _sourceInternalConfiguration.HttpHandlerOptions);
            Assert.Same(
                _sourceInternalConfiguration.LoadBalancerOptions,
                _sourceInternalConfiguration.LoadBalancerOptions);
            Assert.Same(
                _sourceInternalConfiguration.QoSOptions,
                _sourceInternalConfiguration.QoSOptions);
            Assert.Same(
                _sourceInternalConfiguration.ServiceProviderConfiguration,
                _sourceInternalConfiguration.ServiceProviderConfiguration);
        }
    }
}