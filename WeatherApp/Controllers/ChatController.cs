using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using WeatherApp.Models.AIModels;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/ai/chat")]
    public class ChatController : ControllerBase
    {
        private readonly ChatClient _chat;
        private readonly WeatherService _weather;

        // Three predefined prompts, now in English
        private static readonly List<Predef> _questions = new()
        {
            new Predef {
                Key      = "today",
                Template = "Analyze the current weather in {City} for a user with role “{Role}”. Provide a brief report and recommendations."
            },
            new Predef {
                Key      = "week",
                Template = "Analyze the 7-day weather forecast for {City} for a user with role “{Role}”. Highlight key trends."
            },
            new Predef {
                Key      = "history",
                Template = "Analyze historical weather data for {City} from {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} for a user with role “{Role}”. Provide insights on changes."
            },
        };

        public ChatController(ChatClient chatClient, WeatherService weatherService)
        {
            _chat = chatClient;
            _weather = weatherService;
        }

        [HttpPost]
        public async Task<ActionResult<ChatResponse>> Post([FromBody] ChatPayload p)
        {
            var q = _questions.FirstOrDefault(x => x.Key == p.QuestionKey);
            if (q == null)
                return BadRequest("Unknown questionKey");

            if (string.IsNullOrWhiteSpace(p.City))
                return BadRequest("City is required");

            // Substitute common placeholders
            var role = string.IsNullOrWhiteSpace(p.Role) ? "general user" : p.Role;
            var tpl = q.Template
                          .Replace("{City}", p.City)
                          .Replace("{Role}", role)
                          .Replace("{StartDate}", p.StartDate?.ToString("yyyy-MM-dd") ?? "")
                          .Replace("{EndDate}", p.EndDate?.ToString("yyyy-MM-dd") ?? "");

            var prompt = tpl + "\n\n";

            if (p.QuestionKey == "today")
            {
                // use weekly forecast, but only first day
                var wf = await _weather.GetWeeklyForecastAsync(p.City);
                if (wf == null) return BadRequest("Failed to retrieve weekly forecast");

                var firstDay = wf.Days.FirstOrDefault();
                if (firstDay == null) return BadRequest("No data available for any day");

                var todaySummary = new
                {
                    date = firstDay.Date.ToString("yyyy-MM-dd"),
                    description = firstDay.Description,
                    tempDay = firstDay.TemperatureDay,
                    tempNight = firstDay.TemperatureNight,
                    humidity = firstDay.Humidity,
                    windSpeed = firstDay.WindSpeed
                };

                var jsonToday = JsonSerializer.Serialize(todaySummary, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                prompt += "Data for the first day (date, description, tempDay, tempNight, humidity, windSpeed):\n"
                       + jsonToday;
            }
            else if (p.QuestionKey == "week")
            {
                var wf = await _weather.GetWeeklyForecastAsync(p.City);
                if (wf == null) return BadRequest("Failed to retrieve weekly forecast");

                var days = wf.Days
                             .Select(d => new {
                                 date = d.Date.ToString("yyyy-MM-dd"),
                                 min = d.TemperatureNight,
                                 max = d.TemperatureDay
                             });
                var json = JsonSerializer.Serialize(days);
                prompt += "Forecast summary (date, min, max): " + json;
            }
            else // history
            {
                if (p.StartDate == null || p.EndDate == null)
                    return BadRequest("Please specify StartDate and EndDate for historical data");

                var hf = await _weather.GetHistoricalAsync(
                    p.City, p.StartDate.Value, p.EndDate.Value,
                    new[] { "temperature_2m", "relativehumidity_2m" }
                );
                if (hf == null) return BadRequest("Failed to retrieve historical data");

                var combined = hf.Hourly.Time
                    .Select((t, idx) => new {
                        time = t,
                        temp = hf.Hourly.Temperature_2m[idx],
                        hum = hf.Hourly.Relativehumidity_2m[idx]
                    });

                var sample = combined.Take(3).ToList();
                var json = JsonSerializer.Serialize(sample);
                prompt += "Sample data (time, temp, humidity): " + json + " …";
            }

            // Send to GPT
            var compResult = await _chat.CompleteChatAsync(prompt);
            var completion = compResult.Value;
            var reply = completion.Content[0].Text.Trim();

            return Ok(new ChatResponse { Reply = reply });
        }

        private class Predef
        {
            public string Key { get; set; }
            public string Template { get; set; }
        }
    }
}
