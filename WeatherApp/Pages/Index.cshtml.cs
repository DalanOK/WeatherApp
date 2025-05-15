using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WeatherApp.Models;

public class IndexModel : PageModel
{
    private readonly WeatherService _weatherService;
    public WeeklyForecast WeeklyForecast { get; set; }
    public DailyForecast SelectedDay { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? DayIndex { get; set; }

    public IndexModel(WeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task OnGetAsync(string city = "Kyiv", int? dayIndex = null)
    {
        WeeklyForecast = await _weatherService.GetWeeklyForecastAsync(city);
        if (WeeklyForecast?.Days != null && WeeklyForecast.Days.Count > 0)
        {
            SelectedDay = WeeklyForecast.Days[dayIndex ?? 0];
        }
    }
}
