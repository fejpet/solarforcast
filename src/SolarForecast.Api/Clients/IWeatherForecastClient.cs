namespace SolarForecast.Api.Clients;

/// <summary>
/// Abstraction for retrieving hourly solar irradiance forecasts from a weather provider.
/// </summary>
public interface IWeatherForecastClient
{
    /// <summary>
    /// Fetches hourly solar irradiance (W/m²) for the specified date.
    /// </summary>
    /// <param name="date">Target date to retrieve forecasts for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A dictionary or map of time ("HH:mm") to irradiance in W/m².</returns>
    Task<IReadOnlyDictionary<string, double>> GetHourlyIrradianceAsync(
        DateOnly date,
        CancellationToken cancellationToken = default);
}
