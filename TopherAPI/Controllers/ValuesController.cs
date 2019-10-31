using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TopherAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// Returns a string letting you know if the service is up and running.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("health")]
        [ProducesResponseType(typeof(string), 200)]
        public async virtual Task<ObjectResult> HealthPing()
        {
            return StatusCode(200, "Status is good!");
        }

    }
}
