namespace WeatherApp.Models
{
    public class WeatherData
    {
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string TimeZone { get; set; }
        public string LocalTime { get; set; }

        // Из блока "current"
        public double TemperatureC { get; set; }
        public double Temperature { get; set; }
        public double TemperatureF { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }

        public double Humidity { get; set; }
        public double WindKph { get; set; }
        public double WindSpeed { get; set; }
        public double WindMph { get; set; }
        public double GustKph { get; set; }
        public double GustMph { get; set; }

        public double PressureMb { get; set; }
        public double PressureIn { get; set; }
        public double PrecipMm { get; set; }
        public double PrecipIn { get; set; }
        public double UV { get; set; }
    }
}