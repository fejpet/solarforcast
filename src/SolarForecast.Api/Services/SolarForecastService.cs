using System.Globalization;
using Microsoft.Extensions.Options;
using SolarForecast.Api.Clients;
using SolarForecast.Api.Configuration;
using SolarForecast.Api.Models;

namespace SolarForecast.Api.Services;

/// <summary>
/// Default implementation of <see cref="ISolarForecastService"/>.
/// </summary>
public sealed class SolarForecastService : ISolarForecastService
{
    public const string DefaultUnit = "W/m2";
    private static readonly string[] DefaultForecastHours = ["09:00", "11:00", "13:00"];

    private readonly IWeatherForecastClient _weatherClient;
    private readonly IOptionsMonitor<SolarForecastOptions> _optionsMonitor;
    private readonly TimeProvider _timeProvider;

    public SolarForecastService(
        IWeatherForecastClient weatherClient,
        IOptionsMonitor<SolarForecastOptions> optionsMonitor,
        TimeProvider? timeProvider = null)
    {
        _weatherClient = weatherClient;
        _optionsMonitor = optionsMonitor;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public async Task<TomorrowIrradianceResponse> GetTomorrowIrradianceAsync(
        CancellationToken cancellationToken = default)
    {
        var tomorrow = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime.AddDays(1));
        var hourlyData = await _weatherClient.GetHourlyIrradianceAsync(tomorrow, cancellationToken);

        var configuredHours = _optionsMonitor.CurrentValue.ForecastHours;
        IEnumerable<string> rawHours = configuredHours is { Count: > 0 }
            ? configuredHours
            : DefaultForecastHours;

        var targetHours = rawHours
            .Select(NormalizeTimeSlot)
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();

        var forecasts = new List<HourlyIrradianceItem>(targetHours.Count);
        foreach (var hour in targetHours)
        {
            var irradiance = hourlyData.TryGetValue(hour, out var value) ? value : 0.0;
            forecasts.Add(new HourlyIrradianceItem(hour, irradiance));
        }

        return new TomorrowIrradianceResponse(
            Date: tomorrow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Unit: DefaultUnit,
            Forecasts: forecasts);
    }

    private static string NormalizeTimeSlot(string rawTime)
    {
        if (string.IsNullOrWhiteSpace(rawTime))
        {
            return string.Empty;
        }

        if (TimeOnly.TryParse(rawTime.Trim(), CultureInfo.InvariantCulture, out var parsedTime))
        {
            return parsedTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        return rawTime.Trim();
    }
}
