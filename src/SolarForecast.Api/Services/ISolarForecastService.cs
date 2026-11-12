using SolarForecast.Api.Models;

namespace SolarForecast.Api.Services;

/// <summary>
/// Service responsible for building solar irradiance forecasts for configured time slots.
/// </summary>
public interface ISolarForecastService
{
    /// <summary>
    /// Gets the solar irradiance forecast for tomorrow at the configured hours.
    /// </summary>
    Task<TomorrowIrradianceResponse> GetTomorrowIrradianceAsync(CancellationToken cancellationToken = default);
}
