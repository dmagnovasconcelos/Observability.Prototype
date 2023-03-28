using Microsoft.AspNetCore.Mvc;
using Prometheus.Prototype.Api;

namespace Second.Prototype.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

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
            })
                .ToArray());
        }
        catch (Exception ex)
        {
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
            return Unauthorized();
        }

        return Ok(Summaries[rng]);
    }
}