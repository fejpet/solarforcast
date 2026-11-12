# SolarForecast

**SolarForecast** is a .NET 10 Web API application that estimates next-day solar irradiance and photovoltaic (PV) energy production using weather forecast data from [Open-Meteo](https://open-meteo.com/), helping optimize overnight home battery charging during off-peak electricity tariff periods.

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (`10.0.100` or newer)
- Internet access to reach the Open-Meteo API (`https://api.open-meteo.com/v1/forecast`)

---

## Project Structure

```text
solarforcast/
├── AGENT.md                        # AI agent development guidelines
├── README.md                       # Usage and setup documentation
├── SolarForecast.slnx              # .NET solution file
├── docs/
│   ├── 01-requirement/
│   │   └── requirement.md          # Functional & non-functional requirements
│   └── 02-implementation/          # Implementation plans
├── src/
│   └── SolarForecast.Api/          # ASP.NET Core Web API project
└── tests/
    └── SolarForecast.Tests/        # xUnit automated test suite
```

---

## Configuration

The application is configured via [`src/SolarForecast.Api/appsettings.json`](src/SolarForecast.Api/appsettings.json) under the `SolarForecast` section:

```json
{
  "SolarForecast": {
    "BaseUrl": "https://api.open-meteo.com/v1/forecast",
    "Latitude": 47.4979,
    "Longitude": 19.0402,
    "Timezone": "Europe/Budapest",
    "ForecastHours": [
      "09:00",
      "11:00",
      "13:00"
    ]
  }
}
```

### Configuration Options

| Setting | Type | Default | Description |
| :--- | :--- | :--- | :--- |
| `BaseUrl` | `string` | `"https://api.open-meteo.com/v1/forecast"` | Open-Meteo Forecast API endpoint. |
| `Latitude` | `double` | `47.4979` | Geographic latitude of the solar installation. |
| `Longitude` | `double` | `19.0402` | Geographic longitude of the solar installation. |
| `Timezone` | `string` | `"Europe/Budapest"` | IANA timezone name used for hourly timestamps. |
| `ForecastHours` | `string[]` | `["09:00", "11:00", "13:00"]` | Array of `"HH:mm"` times of day to include in the forecast response. Add or remove hours here to customize the returned slots without modifying code. |

You can also override any setting via environment variables using standard ASP.NET Core conventions, for example:

```bash
export SolarForecast__Latitude=47.55
export SolarForecast__Longitude=19.10
export SolarForecast__ForecastHours__0="08:00"
export SolarForecast__ForecastHours__1="10:00"
export SolarForecast__ForecastHours__2="12:00"
export SolarForecast__ForecastHours__3="14:00"
```

---

## Getting Started

### 1. Build the Solution

```bash
dotnet build SolarForecast.slnx
```

### 2. Run the Tests

```bash
dotnet test SolarForecast.slnx
```

### 3. Run the Web API

```bash
dotnet run --project src/SolarForecast.Api
```

By default, the development server listens on `http://localhost:5074` (and `https://localhost:7006` when using the `https` profile).

---

## API Reference

### Get Tomorrow's Solar Irradiance Forecast

Returns tomorrow's forecasted Global Horizontal Irradiance (`shortwave_radiation`) in `W/m²` for the configured time slots.

- **Endpoint:** `GET /api/forecast/tomorrow`
- **Response Type:** `application/json`

#### Example Request

```bash
curl -s http://localhost:5074/api/forecast/tomorrow
```

#### Example Response (`200 OK`)

```json
{
  "date": "2026-11-13",
  "unit": "W/m2",
  "forecasts": [
    {
      "time": "09:00",
      "irradiance": 245.5
    },
    {
      "time": "11:00",
      "irradiance": 410.0
    },
    {
      "time": "13:00",
      "irradiance": 380.2
    }
  ]
}
```
