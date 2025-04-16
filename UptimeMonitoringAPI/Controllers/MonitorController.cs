using Microsoft.AspNetCore.Mvc;
using UptimeMonitor.Services;
using UptimeMonitor.Models;

namespace UptimeMonitoringAPI.Controllers {

    [ApiController]
    [Route("monitor")]
    public class MonitorController : ControllerBase
    {
        private readonly IRedisService _redis;
        public MonitorController(IRedisService redis) => _redis = redis;

        [HttpPost]
        public IActionResult Register([FromBody] MonitorRequest request)
        {
            KafkaProducer.Produce("check-requests", request);
            return Ok(new { message = "URL registered." });
        }

        [HttpGet("status")]
        public IActionResult GetStatus() => Ok(_redis.GetAllStatuses());

        [HttpGet("failures")]
        public IActionResult GetFailures() => Ok(_redis.GetFailures());
    }

}