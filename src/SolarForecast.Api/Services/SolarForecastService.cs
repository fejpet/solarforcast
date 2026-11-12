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
        var options = _optionsMonitor.CurrentValue;

        var targetHours = options.ForecastHours
            .Select(NormalizeTimeSlot)
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();

        var forecasts = new List<HourlyIrradianceItem>(targetHours.Count);
        foreach (var hour in targetHours)
        {
            var irradiance = hourlyData.TryGetValue(hour, out var value) ? value : 0.0;
            forecasts.Add(new HourlyIrradianceItem(hour, irradiance));
        }

        var estimatedProductionKwh = CalculateEstimatedProductionKwh(hourlyData.Values, options);
        var targetBatterySocPercent = CalculateTargetBatterySocPercent(estimatedProductionKwh, options);

        return new TomorrowIrradianceResponse(
            Date: tomorrow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            Unit: DefaultUnit,
            EstimatedProductionKwh: estimatedProductionKwh,
            TargetBatterySocPercent: targetBatterySocPercent,
            Forecasts: forecasts);
    }

    private static double CalculateEstimatedProductionKwh(
        IEnumerable<double> hourlyIrradianceWm2,
        SolarForecastOptions options)
    {
        var totalIrradianceKwhPerM2 = hourlyIrradianceWm2.Sum(irradiance => Math.Max(0.0, irradiance) / 1000.0);
        var rawProductionKwh = totalIrradianceKwhPerM2 * options.SystemPeakPowerKwp * options.PerformanceRatio;
        return Math.Round(rawProductionKwh, 2, MidpointRounding.AwayFromZero);
    }

    private static double CalculateTargetBatterySocPercent(
        double estimatedProductionKwh,
        SolarForecastOptions options)
    {
        var energyDeficitKwh = Math.Max(0.0, options.ExpectedDailyConsumptionKwh - estimatedProductionKwh);
        var additionalSocPercent = options.BatteryCapacityKwh > 0.0
            ? (energyDeficitKwh / options.BatteryCapacityKwh) * 100.0
            : 0.0;

        var unclampedTargetSoc = options.MinSocPercent + additionalSocPercent;
        var clampedTargetSoc = Math.Clamp(unclampedTargetSoc, options.MinSocPercent, options.MaxSocPercent);
        return Math.Round(clampedTargetSoc, 1, MidpointRounding.AwayFromZero);
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
