FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app 
#
# copy csproj and restore as distinct layers

COPY ./*.sln ./
COPY ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromDiscovery/*.csproj ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromDiscovery/
COPY ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromConfig/*.csproj ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromConfig/
COPY ./Example/NihFix.Ocelot.MixedRouting.Example.ApiGateway/*.csproj ./Example/NihFix.Ocelot.MixedRouting.Example.ApiGateway/
COPY ./NihFix.Ocelot.MixedRouting/*.csproj ./NihFix.Ocelot.MixedRouting/
COPY ./NihFix.Ocelot.MixedRouting.Tests/*.csproj ./NihFix.Ocelot.MixedRouting.Tests/

RUN dotnet restore

COPY ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromDiscovery/. ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromDiscovery/
COPY ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromConfig/. ./Example/NihFix.Ocelot.MixedRouting.Example.ApiFromConfig/
COPY ./Example/NihFix.Ocelot.MixedRouting.Example.ApiGateway/. ./Example/NihFix.Ocelot.MixedRouting.Example.ApiGateway/
COPY ./NihFix.Ocelot.MixedRouting/. ./NihFix.Ocelot.MixedRouting/
COPY ./NihFix.Ocelot.MixedRouting.Tests/. ./NihFix.Ocelot.MixedRouting.Tests/

WORKDIR /app/Example/NihFix.Ocelot.MixedRouting.Example.ApiFromConfig

RUN dotnet publish -c Release -o out 

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/Example/NihFix.Ocelot.MixedRouting.Example.ApiFromConfig/out/. ./
ENTRYPOINT ["dotnet", "NihFix.Ocelot.MixedRouting.Example.ApiFromConfig.dll"]

