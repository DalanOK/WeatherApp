using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherApp.Models;
using System;
using System.Collections.Generic;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private const string ApiKey = "d847d396db5b1e691611792866d29a89";
    private const string BaseUrl = "https://api.openweathermap.org/data/2.5/weather";

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}?q={city}&units=metric&appid={ApiKey}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        return new WeatherData
        {
            City = data.GetProperty("name").GetString(),
            Description = data.GetProperty("weather")[0].GetProperty("description").GetString(),
            Temperature = data.GetProperty("main").GetProperty("temp").GetDouble(),
            Humidity = data.GetProperty("main").GetProperty("humidity").GetDouble(),
            WindSpeed = data.GetProperty("wind").GetProperty("speed").GetDouble()
        };
    }
    public async Task<WeeklyForecast> GetWeeklyForecastAsync(string city)
    {
        // Получаем координаты города
        var response = await _httpClient.GetAsync($"{BaseUrl}?q={city}&units=metric&appid={ApiKey}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        double lat = data.GetProperty("coord").GetProperty("lat").GetDouble();
        double lon = data.GetProperty("coord").GetProperty("lon").GetDouble();
        string cityName = data.GetProperty("name").GetString();

        // Получаем прогноз на 7 дней
        var oneCallUrl = $"https://api.openweathermap.org/data/3.0/onecall?lat={lat}&lon={lon}&exclude=current,minutely,hourly,alerts&units=metric&appid={ApiKey}";
        var forecastResponse = await _httpClient.GetAsync(oneCallUrl);
        if (!forecastResponse.IsSuccessStatusCode) return null;

        var forecastJson = await forecastResponse.Content.ReadAsStringAsync();
        var forecastData = JsonSerializer.Deserialize<JsonElement>(forecastJson);

        var days = new List<DailyForecast>();
        foreach (var day in forecastData.GetProperty("daily").EnumerateArray())
        {
            days.Add(new DailyForecast
            {
                Date = DateTimeOffset.FromUnixTimeSeconds(day.GetProperty("dt").GetInt64()).DateTime,
                Description = day.GetProperty("weather")[0].GetProperty("description").GetString(),
                TemperatureDay = day.GetProperty("temp").GetProperty("day").GetDouble(),
                TemperatureNight = day.GetProperty("temp").GetProperty("night").GetDouble(),
                Humidity = day.GetProperty("humidity").GetDouble(),
                WindSpeed = day.GetProperty("wind_speed").GetDouble(),
                Icon = day.GetProperty("weather")[0].GetProperty("icon").GetString()
            });
        }

        return new WeeklyForecast
        {
            City = cityName,
            Days = days
        };
    }
}
