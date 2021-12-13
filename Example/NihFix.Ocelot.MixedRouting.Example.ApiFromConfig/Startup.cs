using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NihFix.Ocelot.MixedRouting.Example.ConsulAgent;

namespace NihFix.Ocelot.MixedRouting.Example.ApiFromConfig
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var consulConfig = _configuration.GetSection(ConsulConfig.Consul).Get<ConsulConfig>();
            services.Configure<ConsulConfig>(_configuration.GetSection(ConsulConfig.Consul));
            services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(consulParams =>
            {
                var address = consulConfig.Address;
                consulParams.Address = new Uri(address);
            }));
            services.AddControllers()
                .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}