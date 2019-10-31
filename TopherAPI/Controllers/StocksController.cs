using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DiscordBot.Services;
using StocksMonitor;
using StocksMonitor.Models;

namespace TopherAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        [HttpGet]
        [Route("MarketInfo")]
        [ProducesResponseType(typeof(MarketInformation), 200)]
        public async virtual Task<ObjectResult> GetMarketInformation()
        {
            return StatusCode(200, "");
        }

        [HttpGet]
        [Route("Test")]
        [ProducesResponseType(typeof(string), 200)]
        public async virtual Task<ObjectResult> TestDiscordBot()
        {
            return StatusCode(200, LoggingHandler.DiscordLog.ToString());
        }
    }
}