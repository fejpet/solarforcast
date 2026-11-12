# Implementation Plan: 01 — Next-Day Solar Irradiance Forecast REST API

- **Status:** Completed
- **Related Requirements:** `FR-01` (`FR-01.1`, `FR-01.2`, `FR-01.3`), `NFR-01`, `NFR-02`, `NFR-03` (see `docs/01-requirement/requirement.md`)

---

## 1. Context & Goal

Implement a .NET ASP.NET Core Web API that fetches tomorrow's hourly solar irradiance forecast (`W/m²`) from Open-Meteo and returns the values for a configurable list of times of day (defaulting to `09:00`, `11:00`, `13:00`) as a JSON object containing an array of time/irradiance pairs.

---

## 2. Technical Design

### 2.1 Solution Structure
- `SolarForecast.Api` — ASP.NET Core Web API project
- `SolarForecast.Tests` — xUnit test project for unit and integration testing

### 2.2 Configuration (`appsettings.json`)
Bind configuration section `"SolarForecast"` to `SolarForecastOptions`:
```json
{
  "SolarForecast": {
    "BaseUrl": "https://api.open-meteo.com/v1/forecast",
    "Latitude": 47.4979,
    "Longitude": 19.0402,
    "Timezone": "Europe/Budapest",
    "ForecastHours": [ "09:00", "11:00", "13:00" ]
  }
}
```

### 2.3 Data Models & Contracts
- **`SolarForecastOptions`**:
  - `BaseUrl` (`string`)
  - `Latitude` (`double`)
  - `Longitude` (`double`)
  - `Timezone` (`string`)
  - `ForecastHours` (`List<string>`) — initialized with default `["09:00", "11:00", "13:00"]`
- **`TomorrowIrradianceResponse`**:
  - `Date` (`string`, `yyyy-MM-dd`)
  - `Unit` (`string`, default `"W/m2"`)
  - `Forecasts` (`IReadOnlyList<HourlyIrradianceItem>`)
- **`HourlyIrradianceItem`**:
  - `Time` (`string`, `"HH:mm"`)
  - `Irradiance` (`double`, value in `W/m²`)

### 2.4 Services & Endpoints
- **`IWeatherForecastClient` / `OpenMeteoClient`**:
  - Typed `HttpClient` calling Open-Meteo with `latitude`, `longitude`, `hourly=shortwave_radiation`, `timezone`, and `start_date`/`end_date` set to tomorrow's date.
- **`ISolarForecastService` / `SolarForecastService`**:
  - Queries `IWeatherForecastClient` for tomorrow's hourly irradiance data and filters/maps the results to the hours configured in `SolarForecastOptions.ForecastHours`.
- **Endpoint (`GET /api/forecast/tomorrow`)**:
  - Returns `200 OK` with `TomorrowIrradianceResponse` in JSON format.

---

## 3. Task Breakdown

- [x] **Task 1:** Initialize .NET solution (`SolarForecast.slnx`), Web API project (`src/SolarForecast.Api`), and xUnit test project (`tests/SolarForecast.Tests`).
- [x] **Task 2:** Define `SolarForecastOptions` and configure default values (`09:00`, `11:00`, `13:00`) in `appsettings.json`.
- [x] **Task 3:** Implement `OpenMeteoClient` (`IWeatherForecastClient`) and response deserialization models for `shortwave_radiation`.
- [x] **Task 4:** Implement `SolarForecastService` (`ISolarForecastService`) to filter tomorrow's hourly data by configured `ForecastHours`.
- [x] **Task 5:** Expose `GET /api/forecast/tomorrow` endpoint in `Program.cs`.
- [x] **Task 6:** Add unit tests for `SolarForecastService` and `OpenMeteoClient` (including custom configured hours and default hours).

---

## 4. Verification Plan

- Run `dotnet build` and `dotnet test` to verify all unit tests pass.
- Start the API and send a `GET /api/forecast/tomorrow` request to verify the returned JSON structure contains the `forecasts` array with `09:00`, `11:00`, and `13:00` values in `W/m2`.
