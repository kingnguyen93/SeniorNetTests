using SeniorNetTests.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeniorNetTests.Services
{
    public interface IWeatherForecastService
    {
        public IEnumerable<WeatherForecast> GetWeatherForecast();
    }
}
