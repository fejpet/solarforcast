namespace SolarForecast.Api.Configuration;

/// <summary>
/// Configuration options for the solar irradiance forecast service.
/// </summary>
public sealed class SolarForecastOptions
{
    public const string SectionName = "SolarForecast";

    /// <summary>
    /// Base URL of the Open-Meteo forecast API.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.open-meteo.com/v1/forecast";

    /// <summary>
    /// Geographic latitude of the solar installation.
    /// </summary>
    public double Latitude { get; set; } = 47.4979;

    /// <summary>
    /// Geographic longitude of the solar installation.
    /// </summary>
    public double Longitude { get; set; } = 19.0402;

    /// <summary>
    /// IANA timezone identifier for hourly timestamps.
    /// </summary>
    public string Timezone { get; set; } = "Europe/Budapest";

    /// <summary>
    /// Requested times of day in "HH:mm" format for which tomorrow's solar irradiance forecast is returned.
    /// Defaults to 09:00, 11:00, and 13:00.
    /// </summary>
    public List<string> ForecastHours { get; set; } = ["09:00", "11:00", "13:00"];
}
