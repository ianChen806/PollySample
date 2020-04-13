using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly;

namespace PollySample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> Get()
        {
            var context = new Polly.Context("8888");
            context["TestValue"] = "test";
            
            var httpClient = _clientFactory.CreateClient("Test");
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "http://www.gooasdasdasdgle.com");
            httpRequestMessage.SetPolicyExecutionContext(context);

            return await httpClient.SendAsync(httpRequestMessage);
        }
    }
}