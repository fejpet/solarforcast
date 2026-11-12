using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using SolarForecast.Api.Configuration;

namespace SolarForecast.Api.Clients;

/// <summary>
/// Weather client implementation using the Open-Meteo Forecast API.
/// </summary>
public sealed class OpenMeteoClient : IWeatherForecastClient
{
    private readonly HttpClient _httpClient;
    private readonly IOptionsMonitor<SolarForecastOptions> _optionsMonitor;

    public OpenMeteoClient(HttpClient httpClient, IOptionsMonitor<SolarForecastOptions> optionsMonitor)
    {
        _httpClient = httpClient;
        _optionsMonitor = optionsMonitor;
    }

    public async Task<IReadOnlyDictionary<string, double>> GetHourlyIrradianceAsync(
        DateOnly date,
        CancellationToken cancellationToken = default)
    {
        var options = _optionsMonitor.CurrentValue;
        var dateString = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var latitude = options.Latitude.ToString(CultureInfo.InvariantCulture);
        var longitude = options.Longitude.ToString(CultureInfo.InvariantCulture);
        var timezone = Uri.EscapeDataString(options.Timezone);

        var requestUri = $"{options.BaseUrl}?latitude={latitude}&longitude={longitude}" +
                         $"&hourly=shortwave_radiation&timezone={timezone}" +
                         $"&start_date={dateString}&end_date={dateString}";

        var response = await _httpClient.GetFromJsonAsync<OpenMeteoForecastResponse>(
            requestUri,
            cancellationToken);

        var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

        if (response?.Hourly?.Time is null || response.Hourly.ShortwaveRadiation is null)
        {
            return result;
        }

        var count = Math.Min(response.Hourly.Time.Count, response.Hourly.ShortwaveRadiation.Count);
        for (var i = 0; i < count; i++)
        {
            var rawTimestamp = response.Hourly.Time[i];
            var irradiance = response.Hourly.ShortwaveRadiation[i] ?? 0.0;

            var timePart = ExtractTimePart(rawTimestamp);
            if (!string.IsNullOrEmpty(timePart))
            {
                result[timePart] = irradiance;
            }
        }

        return result;
    }

    private static string ExtractTimePart(string timestamp)
    {
        if (string.IsNullOrWhiteSpace(timestamp))
        {
            return string.Empty;
        }

        var tIndex = timestamp.IndexOf('T');
        if (tIndex >= 0 && tIndex + 1 < timestamp.Length)
        {
            var timeSpanPart = timestamp[(tIndex + 1)..];
            if (TimeOnly.TryParse(timeSpanPart, CultureInfo.InvariantCulture, out var parsedTime))
            {
                return parsedTime.ToString("HH:mm", CultureInfo.InvariantCulture);
            }
        }

        if (DateTime.TryParse(timestamp, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
        {
            return dateTime.ToString("HH:mm", CultureInfo.InvariantCulture);
        }

        return string.Empty;
    }

    public sealed class OpenMeteoForecastResponse
    {
        [JsonPropertyName("hourly")]
        public OpenMeteoHourlyData? Hourly { get; set; }
    }

    public sealed class OpenMeteoHourlyData
    {
        [JsonPropertyName("time")]
        public List<string>? Time { get; set; }

        [JsonPropertyName("shortwave_radiation")]
        public List<double?>? ShortwaveRadiation { get; set; }
    }
}
