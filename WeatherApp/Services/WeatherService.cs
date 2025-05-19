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
    private const string weatherApiKey = "d4849ee17042412bb35135503251405";
    private const string BaseUrl = "https://api.weatherapi.com/v1/current.json";
    private const string WeekWeatherUrl = "https://api.weatherapi.com/v1/forecast.json";
    private const string FutureWeatherUrl = "https://api.weatherapi.com/v1/future.json";
    private string days = "7";

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData> GetWeatherAsync(string city)
    {
        var response = await _httpClient.GetAsync($"{BaseUrl}?key={weatherApiKey}&q={city}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var location = root.GetProperty("location");
        var current = root.GetProperty("current");
        var condition = current.GetProperty("condition");
        var forecast = root.GetProperty("forecast");


        WeatherData weatherData = new WeatherData
        {
            City = location.GetProperty("name").GetString(),
            Description = condition.GetProperty("text").GetString(),
            Temperature = current.GetProperty("temp_c").GetDouble(),
            Humidity = current.GetProperty("humidity").GetDouble(),
            WindSpeed = current.GetProperty("wind_mph").GetDouble()
        };

        return weatherData;
    }
    public async Task<WeeklyForecast> GetWeeklyForecastAsync(string city)
    {
        var response = await _httpClient.GetAsync($"{WeekWeatherUrl}?q={city}&days={days}&key={weatherApiKey}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var location = root.GetProperty("location");
        var current = root.GetProperty("current");
        var forecastArray = root
            .GetProperty("forecast")
            .GetProperty("forecastday")
            .EnumerateArray();

        // Заполняем базовый объект
        var weekly = new WeeklyForecast
        {
            City = location.GetProperty("name").GetString(),
            CurrentWeather = new WeatherData
            {
                City = location.GetProperty("name").GetString(),
                Region = location.GetProperty("region").GetString(),
                Country = location.GetProperty("country").GetString(),
                LocalTime = location.GetProperty("localtime").GetString(),

                TemperatureC = current.GetProperty("temp_c").GetDouble(),
                TemperatureF = current.GetProperty("temp_f").GetDouble(),
                Description = current.GetProperty("condition").GetProperty("text").GetString(),
                IconUrl = current.GetProperty("condition").GetProperty("icon").GetString(),

                Humidity = current.GetProperty("humidity").GetDouble(),
                WindKph = current.GetProperty("wind_kph").GetDouble(),
                WindMph = current.GetProperty("wind_mph").GetDouble(),

                PressureMb = current.GetProperty("pressure_mb").GetDouble(),
                PrecipMm = current.GetProperty("precip_mm").GetDouble(),
                UV = current.GetProperty("uv").GetDouble()
            },
            Days = new List<DailyForecast>()
        };

        // Парсим каждый день
        foreach (var dayElem in forecastArray)
        {
            var dayInfo = dayElem.GetProperty("day");
            var daily = new DailyForecast
            {
                Date = DateTime.Parse(dayElem.GetProperty("date").GetString()),
                Description = dayInfo.GetProperty("condition").GetProperty("text").GetString(),
                TemperatureDay = dayInfo.GetProperty("maxtemp_c").GetDouble(),
                TemperatureNight = dayInfo.GetProperty("mintemp_c").GetDouble(),
                Humidity = dayInfo.GetProperty("avghumidity").GetDouble(),
                WindSpeed = dayInfo.GetProperty("maxwind_kph").GetDouble(),
                Icon = dayInfo.GetProperty("condition").GetProperty("icon").GetString(),
                HourlyForecasts = new List<HourlyForecast>()
            };

            // Парсим почасовой прогноз
            foreach (var hourElem in dayElem.GetProperty("hour").EnumerateArray())
            {
                var hr = new HourlyForecast
                {
                    Date = DateTime.Parse(hourElem.GetProperty("time").GetString()),
                    Description = hourElem.GetProperty("condition").GetProperty("text").GetString(),
                    TemperatureDay = hourElem.GetProperty("temp_c").GetDouble(),
                    Humidity = hourElem.GetProperty("humidity").GetDouble(),
                    WindSpeed = hourElem.GetProperty("wind_kph").GetDouble(),
                    Icon = hourElem.GetProperty("condition").GetProperty("icon").GetString()
                };
                daily.HourlyForecasts.Add(hr);
            }

            weekly.Days.Add(daily);
        }

        return weekly;
    }

    public async Task<WeeklyForecast> GetFutureForecastAsync(string city, string date)
    {
        var response = await _httpClient.GetAsync($"{FutureWeatherUrl}?q={city}&dt={date}&key={weatherApiKey}");
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Информация о локации
        var location = root.GetProperty("location");

        // Массив дней прогноза
        var forecastDays = root
            .GetProperty("forecast")
            .GetProperty("forecastday")
            .EnumerateArray();

        // Создаём начальную модель
        var weekly = new WeeklyForecast
        {
            City = location.GetProperty("name").GetString(),
            CurrentWeather = new WeatherData
            {
                Region = location.GetProperty("region").GetString(),
                Country = location.GetProperty("country").GetString(),
                LocalTime = location.GetProperty("localtime").GetString()
            },
            Days = new List<DailyForecast>()
        };

        // Пробегаем по каждому дню
        foreach (var dayElem in forecastDays)
        {
            // Парсим основной блок "day"
            var dayInfo = dayElem.GetProperty("day");
            var daily = new DailyForecast
            {
                Date = DateTime.Parse(dayElem.GetProperty("date").GetString()!),
                Description = dayInfo.GetProperty("condition").GetProperty("text").GetString()!,
                TemperatureDay = dayInfo.GetProperty("maxtemp_c").GetDouble(),
                TemperatureNight = dayInfo.GetProperty("mintemp_c").GetDouble(),
                Humidity = dayInfo.GetProperty("avghumidity").GetDouble(),
                WindSpeed = dayInfo.GetProperty("maxwind_kph").GetDouble(),
                Icon = dayInfo.GetProperty("condition").GetProperty("icon").GetString()!,
                HourlyForecasts = new List<HourlyForecast>()
            };

            // Парсим почасовой прогноз
            foreach (var hourElem in dayElem.GetProperty("hour").EnumerateArray())
            {
                var hr = new HourlyForecast
                {
                    Date = DateTime.Parse(hourElem.GetProperty("time").GetString()!),
                    Description = hourElem.GetProperty("condition").GetProperty("text").GetString()!,
                    TemperatureDay = hourElem.GetProperty("temp_c").GetDouble(),
                    Humidity = hourElem.GetProperty("humidity").GetDouble(),
                    WindSpeed = hourElem.GetProperty("wind_kph").GetDouble(),
                    Icon = hourElem.GetProperty("condition").GetProperty("icon").GetString()!
                };
                daily.HourlyForecasts.Add(hr);
            }

            weekly.Days.Add(daily);
        }

        return weekly;
    }
}
