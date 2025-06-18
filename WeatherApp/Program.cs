using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Chat;
using WeatherApp.Data;
using WeatherApp.Models;
using WeatherApp.Models.AIModels;
using FluentScheduler;
using WeatherApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddIdentity<Users, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddHttpClient<WeatherService>();
builder.Services.AddTransient<EmailService>();

builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var apiKey = cfg["OpenAI:ApiKey"];
    return new ChatClient(
        model: "gpt-3.5-turbo",  
        apiKey: apiKey
    );
});

builder.Services.Configure<OpenWeatherOptions>(
    builder.Configuration.GetSection("OpenWeatherMap"));

var app = builder.Build();

JobManager.Initialize(new NotificationRegistry(app.Services, app.Configuration));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
