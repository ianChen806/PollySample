using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;

namespace PollySample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddPolicyRegistry(new PolicyRegistry()
            {
                {
                    "No", HttpPolicyExtensions.HandleTransientHttpError()
                                              .CircuitBreakerAsync(1,
                                                                   TimeSpan.FromSeconds(10))
                },
                {
                    "Test", Policy.BulkheadAsync<HttpResponseMessage>(1)
                },
            });

            services.AddHttpClient("Test")
                    .AddPolicyHandlerFromRegistry("Test");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}