using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DelayApiServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OkController : ControllerBase
    {
        private static readonly Random _random = new Random();

        // GET api/ok
        [HttpGet]
        public ActionResult Get()
        {
            return Ok();
        }

        // GET api/ok/3000/5000
        [HttpGet("{minDelay}/{maxDelay}")]
        public async Task<ActionResult> Get(int minDelay, int maxDelay)
        {
            await Task.Delay(_random.Next(minDelay, maxDelay));
            return Ok();
        }
    }
}