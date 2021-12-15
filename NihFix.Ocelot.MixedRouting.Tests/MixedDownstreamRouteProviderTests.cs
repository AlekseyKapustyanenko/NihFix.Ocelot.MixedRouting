using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NSubstitute.Core.Arguments;
using Ocelot.Configuration;
using Ocelot.Configuration.File;
using Ocelot.DownstreamRouteFinder;
using Ocelot.DownstreamRouteFinder.Finder;
using Ocelot.DownstreamRouteFinder.UrlMatcher;
using Ocelot.Errors;
using Ocelot.Responses;
using Ocelot.Values;
using Xunit;

namespace NihFix.Ocelot.MixedRouting.Tests
{
    public class MixedDownstreamRouteProviderTests
    {
        private readonly IDownstreamRouteProvider _finderRouteProvider;
        private readonly IDownstreamRouteProvider _creatorRouteProvider;
        private readonly MixedDownstreamRouteProvider _mixedRouteProvider;
        private readonly IInternalConfiguration _configuration;
        private OkResponse<DownstreamRoute> _okResponse;
        private ErrorResponse<DownstreamRoute> _errorResponse;

        public MixedDownstreamRouteProviderTests()
        {
            _finderRouteProvider = Substitute.For<IDownstreamRouteProvider>();
            _creatorRouteProvider = Substitute.For<IDownstreamRouteProvider>();
            _mixedRouteProvider = MixedDownstreamRouteProvider.Create(
                new Dictionary<string, IDownstreamRouteProvider>
                {
                    {nameof(DownstreamRouteFinder), _finderRouteProvider},
                    {nameof(DownstreamRouteCreator), _creatorRouteProvider}
                });
            _configuration = Substitute.For<IInternalConfiguration>();
            _okResponse = new OkResponse<DownstreamRoute>(
                new DownstreamRoute(new List<PlaceholderNameAndValue>(),
                    new ReRoute(new List<DownstreamReRoute>(),
                        new List<AggregateReRouteConfig>(),
                        new List<HttpMethod>(),
                        new UpstreamPathTemplate("template", 0, true, "originalValue"),
                        "host",
                        "aggregator")));
            _errorResponse = new ErrorResponse<DownstreamRoute>((Error) null);
        }

        [Fact]
        public async Task Get_FindRouteInFinder_ShouldCallFinder()
        {
            _finderRouteProvider.Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>())
                .Returns(_okResponse);

            var result = _mixedRouteProvider.Get(
                string.Empty,
                string.Empty,
                string.Empty,
                _configuration,
                string.Empty);
            Assert.Equal(_okResponse, result);
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

        [Fact]
        public async Task Get_NotFindRouteInFinder_ShouldCallCreator()
        {
            _finderRouteProvider.Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>())
                .Returns(_errorResponse);
            _creatorRouteProvider.Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>())
                .Returns(_okResponse);

            var result = _mixedRouteProvider.Get(
                string.Empty,
                string.Empty,
                string.Empty,
                _configuration,
                string.Empty);
            Assert.Equal(_okResponse, result);
            _finderRouteProvider
                .Received(1)
                .Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>());
            _creatorRouteProvider
                .Received(1)
                .Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>());
        }

        [Fact]
        public async Task Get_NotFoundRouteEveryWhere_ShouldHaveBothErrorsInResponse()
        {
            _finderRouteProvider.Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>())
                .Returns(_errorResponse);
            _creatorRouteProvider.Get(Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<string>(),
                    Arg.Any<IInternalConfiguration>(),
                    Arg.Any<string>())
                .Returns(_errorResponse);
            var result = _mixedRouteProvider.Get(
                string.Empty,
                string.Empty,
                string.Empty,
                _configuration,
                string.Empty);
            Assert.Equal(2, result.Errors.Count);
        }
    }
}