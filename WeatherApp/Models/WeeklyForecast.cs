using System.Collections.Generic;
using WeatherApp.Models;

namespace WeatherApp.Models
{
    public class WeeklyForecast
    {
        public string City { get; set; }

        public WeatherData CurrentWeather { get; set; }
        public List<DailyForecast> Days { get; set; }
    }
}
