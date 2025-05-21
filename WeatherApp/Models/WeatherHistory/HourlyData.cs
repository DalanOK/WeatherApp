namespace WeatherApp.Models.WeatherHistory
{
    public class HourlyData
    {
        public List<DateTime> Time { get; set; }
        public List<double> Temperature_2m { get; set; }
        public List<double> Relativehumidity_2m { get; set; }
        public List<double> Windspeed_10m { get; set; }
        public List<double> Precipitation { get; set; }
        public List<double> Pressure_msl { get; set; }

    }
}
