namespace SolarForecast.Api.Configuration;

/// <summary>
/// Configuration options for the solar irradiance forecast and battery target SoC calculator.
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
    /// Configured via appsettings.json (defaulting to 09:00, 11:00, and 13:00).
    /// </summary>
    public List<string> ForecastHours { get; set; } = [];

    /// <summary>
    /// Installed peak photovoltaic capacity in kWp.
    /// </summary>
    public double SystemPeakPowerKwp { get; set; } = 5.0;

    /// <summary>
    /// Overall PV system performance ratio / efficiency (0.0 to 1.0).
    /// </summary>
    public double PerformanceRatio { get; set; } = 0.8;

    /// <summary>
    /// Expected household daily energy consumption in kWh.
    /// </summary>
    public double ExpectedDailyConsumptionKwh { get; set; } = 10.0;

    /// <summary>
    /// Total home battery storage capacity in kWh.
    /// </summary>
    public double BatteryCapacityKwh { get; set; } = 10.0;

    /// <summary>
    /// Minimum allowed battery State of Charge percentage (0 to 100).
    /// </summary>
    public double MinSocPercent { get; set; } = 20.0;

    /// <summary>
    /// Maximum allowed battery State of Charge percentage (0 to 100).
    /// </summary>
    public double MaxSocPercent { get; set; } = 100.0;
}
