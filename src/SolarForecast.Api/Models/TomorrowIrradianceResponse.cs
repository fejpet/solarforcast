using System.Text.Json.Serialization;

namespace SolarForecast.Api.Models;

/// <summary>
/// Represents the next-day solar irradiance forecast, estimated PV production, and target overnight battery SoC.
/// </summary>
public sealed record TomorrowIrradianceResponse(
    [property: JsonPropertyName("date")] string Date,
    [property: JsonPropertyName("unit")] string Unit,
    [property: JsonPropertyName("estimatedProductionKwh")] double EstimatedProductionKwh,
    [property: JsonPropertyName("targetBatterySocPercent")] double TargetBatterySocPercent,
    [property: JsonPropertyName("forecasts")] IReadOnlyList<HourlyIrradianceItem> Forecasts);

/// <summary>
/// Represents a single time slot's solar irradiance forecast in W/m².
/// </summary>
public sealed record HourlyIrradianceItem(
    [property: JsonPropertyName("time")] string Time,
    [property: JsonPropertyName("irradiance")] double Irradiance);
