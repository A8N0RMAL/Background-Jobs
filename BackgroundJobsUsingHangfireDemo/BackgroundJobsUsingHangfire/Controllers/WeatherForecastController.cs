using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundJobsUsingHangfire.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // 1. Enqueue Job Example
        [HttpPost("enqueue")]
        public IActionResult EnqueueJob()
        {
            // Enqueue a background job to send an email
            BackgroundJob.Enqueue(() => SendEmail("LuciferXD@outlook.com"));
            return Ok("Job enqueued!");
        }

        // 2. Scheduled Job Example
        [HttpPost("schedule")]
        public IActionResult ScheduleJob()
        {
            Console.WriteLine(DateTime.Now);
            // Schedule a background job to send an email after 3 minutes
            BackgroundJob.Schedule(() => SendEmail("LuciferXD@outlook.com"), TimeSpan.FromMinutes(3));
            return Ok("Job scheduled!");
        }

        // 3. Recurring Job Example
        [HttpPost("start-recurring")]
        public IActionResult StartRecurringUpdate()
        {
            // Start a recurring job to send an email every minute
            RecurringJob.AddOrUpdate(
                "send-email-minutely",
                () => SendEmail("LuciferXD@outlook.com"),
                Cron.Minutely);
            return Ok("Recurring job started!");
        }

        [ApiExplorerSettings(IgnoreApi = true)] // Ignore this method in API documentation
        public void SendEmail(string email)
        {
            Console.WriteLine($"Email sent to {email} at {DateTime.Now}");
        }
    }
}
