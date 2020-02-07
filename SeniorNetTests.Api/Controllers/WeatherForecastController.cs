using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SeniorNetTests.Services;

namespace SeniorNetTests.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(IWeatherForecastService weatherForecastService, ILogger<WeatherForecastController> logger)
        {
            _weatherForecastService = weatherForecastService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable Get()
        {
            try
            {
                var test = MethodBase.GetCurrentMethod().Name;
                return _weatherForecastService.GetWeatherForecast();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: ", MethodBase.GetCurrentMethod().Name, ex.GetBaseException().Message);
                return default;
            }
        }
    }
}
