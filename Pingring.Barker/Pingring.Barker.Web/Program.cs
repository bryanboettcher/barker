using Pingring.Barker.Web.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (Environment.GetEnvironmentVariable("HOME") == "/home/insta")
    builder.Services.AddSingleton<IGpioMonitor, RpiGpioMonitor>();
else
    builder.Services.AddSingleton<IGpioMonitor, FakeGpioMonitor>();

builder.Services.AddHostedService<SoundMonitorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/weatherforecast", () => new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 22, "Perfectly average"))
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
