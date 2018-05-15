using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DelayApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HttpClientController : ControllerBase
    {
        private static readonly Random _random = new Random();
        private static readonly HttpClient _httpClient = new HttpClient();

        // GET api/httpclient
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            await _httpClient.GetAsync(GetOutboundUri());

            return Ok();
        }

        // GET api/httpclient/3000/5000
        [HttpGet("{minDelay}/{maxDelay}")]
        public async Task<ActionResult> Get(int minDelay, int maxDelay)
        {
            await Task.Delay(_random.Next(minDelay, maxDelay));

            await _httpClient.GetAsync(GetOutboundUri());

            return Ok();
        }

        // GET api/httpclient/3000/5000/3000/5000
        [HttpGet("{minDelay}/{maxDelay}/{minServerDelay}/{maxServerDelay}")]
        public async Task<ActionResult> Get(int minDelay, int maxDelay, int minServerDelay, int maxServerDelay)
        {
            await Task.Delay(_random.Next(minDelay, maxDelay));

            await _httpClient.GetAsync(GetOutboundUri(minServerDelay, maxServerDelay));

            return Ok();
        }

        private Uri GetOutboundUri()
        {
            return (new UriBuilder(Request.Scheme, Request.Host.Host, (int)Request.Host.Port, "/api/ok")).Uri;
        }

        private Uri GetOutboundUri(int minServerDelay, int maxServerDelay)
        {
            return (new UriBuilder(Request.Scheme, Request.Host.Host, (int)Request.Host.Port, $"/api/ok/{minServerDelay}/{maxServerDelay}")).Uri;
        }
    }
}
