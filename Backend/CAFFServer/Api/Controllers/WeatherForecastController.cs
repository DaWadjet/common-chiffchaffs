using Application.Eventing.Command.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
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
        private readonly IMediator mediator;
        private readonly IIdentityService identityService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMediator mediator, IIdentityService identityService)
        {
            _logger = logger;
            this.mediator = mediator;
            this.identityService = identityService;
        }

        [HttpGet("normal")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Authorize(Policy = "User")]
        [HttpGet("authorized")]
        public IEnumerable<WeatherForecast> GetAuthorized()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("authorized-admin")]
        public IEnumerable<WeatherForecast> GetAdmin()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [Authorize(JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("token")]
        public Guid GetId()
        {
            return identityService.GetCurrentUserId();
        }


        [HttpGet("tesztNative")]
        public async Task<ActionResult<string>> TesztNativeComponent()
        {
            var result = await mediator.Send(new FirstCommand(), HttpContext.RequestAborted);
            return Ok(result);
        }
    }
}