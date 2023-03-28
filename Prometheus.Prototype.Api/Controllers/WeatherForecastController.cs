using Microsoft.AspNetCore.Mvc;
using Prometheus.Prototype.Api;

using ILogger = Serilog.ILogger;

namespace Second.Prototype.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger _logger;

    public WeatherForecastController(ILogger logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IActionResult Get()
    {
        try
        {
            var rng = new Random();
            if (rng.Next(0, 5) > 2)
            {
                throw new Exception("Oops what happened?");
            }

            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            }).ToArray());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Something bad happened");
            return StatusCode(500, ex);
        }
    }

    [HttpGet("city")]
    public IActionResult City()
    {
        var rng = new Random().Next(0, 9);
        Thread.Sleep(TimeSpan.FromSeconds(rng + 1));
        return Ok(Summaries[rng]);
    }

    [HttpGet("country")]
    public IActionResult Country()
    {
        var rng = new Random().Next(0, 9);
        if (rng > 5)
        {
            _logger.Warning("Access Unauthorized");
            return Unauthorized();
        }

        return Ok(Summaries[rng]);
    }
}