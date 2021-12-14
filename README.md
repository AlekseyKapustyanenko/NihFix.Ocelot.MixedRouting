# NihFix.Ocelot.MixedRouting
This is extension for [Ocelot](https://github.com/ThreeMammals/Ocelot) API gateway which allow combine routes rules defined in config and [dynamic routing](https://ocelot.readthedocs.io/en/latest/features/servicediscovery.html#dynamic-routing) with service discovery.

## Installation
Install NihFix.Ocelot.MixedRouting and it's dependencies using NuGet.

`Install-Package NihFix.Ocelot.MixedRouting`

Or via the .NET Core CLI:

`dotnet add package NihFix.Ocelot.MixedRouting`

Then call extension method `AddMixedRouting()` after `Ocelot` registration at `IServiceCollection` in `Startup.cs`. As in example below:

```C#
 public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOcelot()
                .AddConsul()//or onather service discovery
                .AddMixedRouting();// this line enables mixedrouting
        }
```

## How it works
This extension change `Ocelot` behaviour. Default one is:
- if Routes doesn't defined and service discovery is used it uses only dynamic routing
- in all other cases it uses dynamic routing

After adding mixed routing it would has such behavior:
- if Routes are defined in config and used service discovery it tries to find suitable route in config if it doesn't find there it uses dynamic routing
- if Routes service discovery isn't used it use only config routes
- in all other cases it uses dynamic routing
