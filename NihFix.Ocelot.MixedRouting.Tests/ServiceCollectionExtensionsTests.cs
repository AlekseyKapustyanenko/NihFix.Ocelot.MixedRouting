using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Ocelot.DependencyInjection;
using Ocelot.DownstreamRouteFinder.Finder;
using Xunit;

namespace NihFix.Ocelot.MixedRouting.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        private ServiceProvider _serviceProvider;

        public ServiceCollectionExtensionsTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(Substitute.For<IConfiguration>());
            serviceCollection.AddOcelot().AddMixedRouting();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [Fact]
        public async Task
            AddMixedRouting_ResolveIDownstreamRouteProviderFactory_ShouldBeMixedDownstreamRouteProviderFactory()
        {
            var service = _serviceProvider.GetRequiredService<IDownstreamRouteProviderFactory>();

            Assert.IsType<MixedDownstreamRouteProviderFactory>(service);
        }

        [Fact]
        public async Task
            AddMixedRouting_ResolveIDownstreamRouteProviderList_ShouldContainsMixedDownstreamRouteProvider()
        {
            var providers = _serviceProvider.GetServices<IDownstreamRouteProvider>();

            var mixedProvider = providers.FirstOrDefault(p => p.GetType() == typeof(MixedDownstreamRouteProvider));
            Assert.NotNull(mixedProvider);
        }
    }
}