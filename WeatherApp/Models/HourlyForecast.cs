﻿namespace WeatherApp.Models
{
    public class HourlyForecast
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double TemperatureDay { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Icon { get; set; }
    }
}
