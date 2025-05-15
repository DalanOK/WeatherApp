using WeatherApp.Models;

public class DailyForecast
{
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public double TemperatureDay { get; set; }
    public double TemperatureNight { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public string Icon { get; set; }
    public List<HourlyForecast> HourlyForecasts { get; set; } = new List<HourlyForecast>();
}
