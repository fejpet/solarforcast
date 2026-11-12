using SolarForecast.Api.Clients;
using SolarForecast.Api.Configuration;
using SolarForecast.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SolarForecastOptions>(
    builder.Configuration.GetSection(SolarForecastOptions.SectionName));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpClient<IWeatherForecastClient, OpenMeteoClient>();
builder.Services.AddScoped<ISolarForecastService, SolarForecastService>();

var app = builder.Build();

app.MapGet("/api/forecast/tomorrow", async (
    ISolarForecastService forecastService,
    CancellationToken cancellationToken) =>
{
    var forecast = await forecastService.GetTomorrowIrradianceAsync(cancellationToken);
    return Results.Ok(forecast);
})
.WithName("GetTomorrowIrradianceForecast");

app.Run();

public partial class Program;
