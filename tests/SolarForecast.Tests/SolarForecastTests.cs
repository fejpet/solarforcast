using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using SolarForecast.Api.Clients;
using SolarForecast.Api.Configuration;
using SolarForecast.Api.Services;

namespace SolarForecast.Tests;

public sealed class SolarForecastTests
{
    [Fact]
    public async Task GetTomorrowIrradianceAsync_UsesConfiguredHours_ReturnsExpectedItems()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 11, 12, 20, 0, 0, TimeSpan.FromHours(1)));
        var options = new SolarForecastOptions
        {
            ForecastHours = ["09:00", "11:00", "13:00"]
        };
        var stubClient = new StubWeatherForecastClient(new Dictionary<string, double>
        {
            ["08:00"] = 110.0,
            ["09:00"] = 245.5,
            ["11:00"] = 410.0,
            ["13:00"] = 380.2,
            ["15:00"] = 150.0
        });

        var service = new SolarForecastService(stubClient, new StaticOptionsMonitor(options), fakeTime);

        // Act
        var result = await service.GetTomorrowIrradianceAsync();

        // Assert
        Assert.Equal("2026-11-13", result.Date);
        Assert.Equal("W/m2", result.Unit);
        Assert.Equal(3, result.Forecasts.Count);

        Assert.Equal("09:00", result.Forecasts[0].Time);
        Assert.Equal(245.5, result.Forecasts[0].Irradiance);

        Assert.Equal("11:00", result.Forecasts[1].Time);
        Assert.Equal(410.0, result.Forecasts[1].Irradiance);

        Assert.Equal("13:00", result.Forecasts[2].Time);
        Assert.Equal(380.2, result.Forecasts[2].Irradiance);
    }

    [Fact]
    public async Task GetTomorrowIrradianceAsync_SupportsExtendedHoursFromConfig()
    {
        // Arrange
        var fakeTime = new FakeTimeProvider(new DateTimeOffset(2026, 11, 12, 20, 0, 0, TimeSpan.FromHours(1)));
        var options = new SolarForecastOptions
        {
            ForecastHours = ["9:00", "11:00", "13:00", "15:00"]
        };
        var stubClient = new StubWeatherForecastClient(new Dictionary<string, double>
        {
            ["09:00"] = 200.0,
            ["11:00"] = 350.0,
            ["13:00"] = 320.0,
            ["15:00"] = 140.0
        });

        var service = new SolarForecastService(stubClient, new StaticOptionsMonitor(options), fakeTime);

        // Act
        var result = await service.GetTomorrowIrradianceAsync();

        // Assert
        Assert.Equal(4, result.Forecasts.Count);
        Assert.Equal("09:00", result.Forecasts[0].Time);
        Assert.Equal("15:00", result.Forecasts[3].Time);
        Assert.Equal(140.0, result.Forecasts[3].Irradiance);
    }

    [Fact]
    public async Task OpenMeteoClient_ParsesShortwaveRadiationResponse()
    {
        // Arrange
        const string jsonResponse = """
        {
          "hourly": {
            "time": [
              "2026-11-13T09:00",
              "2026-11-13T11:00",
              "2026-11-13T13:00"
            ],
            "shortwave_radiation": [
              215.4,
              398.1,
              360.7
            ]
          }
        }
        """;

        var handler = new FakeHttpMessageHandler(jsonResponse);
        using var httpClient = new HttpClient(handler);
        var options = new SolarForecastOptions();
        var client = new OpenMeteoClient(httpClient, new StaticOptionsMonitor(options));

        // Act
        var data = await client.GetHourlyIrradianceAsync(new DateOnly(2026, 11, 13));

        // Assert
        Assert.Equal(215.4, data["09:00"]);
        Assert.Equal(398.1, data["11:00"]);
        Assert.Equal(360.7, data["13:00"]);
    }

    private sealed class StubWeatherForecastClient(IReadOnlyDictionary<string, double> data) : IWeatherForecastClient
    {
        public Task<IReadOnlyDictionary<string, double>> GetHourlyIrradianceAsync(
            DateOnly date,
            CancellationToken cancellationToken = default) => Task.FromResult(data);
    }

    private sealed class StaticOptionsMonitor(SolarForecastOptions currentValue) : IOptionsMonitor<SolarForecastOptions>
    {
        public SolarForecastOptions CurrentValue => currentValue;
        public SolarForecastOptions Get(string? name) => currentValue;
        public IDisposable? OnChange(Action<SolarForecastOptions, string?> listener) => null;
    }

    private sealed class FakeTimeProvider(DateTimeOffset localNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => localNow.ToUniversalTime();
        public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.CreateCustomTimeZone(
            "Custom",
            localNow.Offset,
            "Custom",
            "Custom");
    }

    private sealed class FakeHttpMessageHandler(string responseJson) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}
