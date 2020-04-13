using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
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

            var policySelector = Policy.Handle<HttpRequestException>()
                                       .OrResult<HttpResponseMessage>(r => true)
                                       .WaitAndRetryAsync(new[]
                                       {
                                           TimeSpan.FromSeconds(1), 
                                           TimeSpan.FromSeconds(3), 
                                           TimeSpan.FromSeconds(5), 
                                       });
            var noOpPolicy = Policy.NoOpAsync().AsAsyncPolicy<HttpResponseMessage>();
            services.AddHttpClient("Test", client =>
                    {
                    })
                    .AddPolicyHandler(message => message.Method == HttpMethod.Get
                                          ? policySelector
                                          : noOpPolicy);
        }

        private bool TestPredicate(HttpResponseMessage message)
        {
            return true;
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