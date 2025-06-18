using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;
using WeatherApp.Data;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class NotificationRegistry : Registry
    {
        public NotificationRegistry(IServiceProvider services, IConfiguration config)
        {
            var minutes = int.TryParse(config["EmailNotifications:IntervalMinutes"], out var m) ? m : 1440;
            Schedule(async () => await NotifyUsers(services)).ToRunNow().AndEvery(minutes).Minutes();
        }

        private static async Task NotifyUsers(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var weather = scope.ServiceProvider.GetRequiredService<WeatherService>();
            var email = scope.ServiceProvider.GetRequiredService<EmailService>();

            var users = await db.Users.Where(u => u.ReceiveNotifications && !string.IsNullOrEmpty(u.Email)).ToListAsync();

            foreach (var user in users)
            {
                var cityQuery = string.IsNullOrWhiteSpace(user.Country) ? user.City : $"{user.City},{user.Country}";
                var forecast = await weather.GetWeeklyForecastAsync(cityQuery);
                var today = forecast?.Days?.FirstOrDefault();
                var text = today != null
                    ? $"Прогноз погоды на сегодня для {cityQuery}: {today.Description}. Днём {today.TemperatureDay}°C, ночью {today.TemperatureNight}°C."
                    : $"Не удалось получить прогноз для {cityQuery}.";

                await email.SendEmailAsync(user.Email!, "Прогноз погоды", text);
            }
        }
    }
}