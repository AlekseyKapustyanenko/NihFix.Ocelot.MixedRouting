using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Core.Arguments;
using Ocelot.Configuration;
using Ocelot.DownstreamRouteFinder;
using Ocelot.DownstreamRouteFinder.Finder;
using Ocelot.Responses;
using Xunit;

namespace NihFix.Ocelot.MixedRouting.Tests
{
    public class MixedDownstreamRouteProviderTests
    {
        private IDownstreamRouteProvider _finderRouteProvider;
        private IDownstreamRouteProvider _creatorRouteProvider;
        private MixedDownstreamRouteProvider _mixedRouteProvider;
        private IInternalConfiguration _configuration;

        public MixedDownstreamRouteProviderTests()
        {
            _finderRouteProvider = Substitute.For<IDownstreamRouteProvider>();
            _creatorRouteProvider = Substitute.For<IDownstreamRouteProvider>();
            _mixedRouteProvider = new MixedDownstreamRouteProvider(_finderRouteProvider, _creatorRouteProvider);
            _configuration = Substitute.For<IInternalConfiguration>();
        }

        [Fact]
        public async Task Get_FindRouteInFinder_ShouldCallFinder()
        {
            _finderRouteProvider.Get(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IInternalConfiguration>(),
                Arg.Any<string>())
                .Returns(new OkResponse<DownstreamRoute>(new DownstreamRoute()));
            
            var result = _mixedRouteProvider.Get(
                string.Empty,
                string.Empty,
                string.Empty, 
                _configuration,
                string.Empty);
            Assert.False(result.IsError);
            _finderRouteProvider.Received(1).Get(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IInternalConfiguration>(),
                Arg.Any<string>());
            _creatorRouteProvider.DidNotReceiveWithAnyArgs().Get(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IInternalConfiguration>(),
                Arg.Any<string>());
        }
    }
}