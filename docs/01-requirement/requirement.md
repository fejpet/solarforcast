# Project Requirements — SolarForecast

## 1. Overview

**SolarForecast** is a solar energy production forecast calculator designed to estimate next-day photovoltaic (PV) energy generation using external weather forecast services. Based on the predicted solar production, the system calculates the optimal overnight battery target State of Charge (SoC) so that the home battery can be pre-charged from the grid during off-peak (discounted tariff) hours to cover expected energy deficits.

### Key Objectives
- **Next-Day Solar Production Estimation:** Fetch weather forecast data (e.g., solar irradiance, cloud cover, temperature) from external weather providers to predict tomorrow's solar energy yield.
- **Overnight Battery Target SoC Calculation:** Determine how much energy the battery needs to be charged to overnight, balancing expected household consumption against forecasted solar production.
- **Off-Peak Tariff Optimization:** Enable pre-charging the battery during discounted nighttime electricity tariff windows when the next day's solar generation is predicted to be insufficient, while avoiding unnecessary grid charging when ample solar energy is expected.

---

## 2. Functional Requirements

### FR-01: Next-Day Solar Irradiance Forecast REST API
Provide a REST API endpoint to query the next day's solar irradiance forecast in `W/m²` for configurable time slots.

- **FR-01.1 (Weather Data Source):** Fetch next-day hourly solar irradiance (`shortwave_radiation`, measured in `W/m²`) from the Open-Meteo API based on configured location coordinates (`Latitude`, `Longitude`, `Timezone`).
- **FR-01.2 (Configurable Forecast Time Slots):** Read the requested time slots from the application configuration file (`appsettings.json`). The default configured times must be `09:00`, `11:00`, and `13:00`.
- **FR-01.3 (Extensible JSON Response Structure):** Return a JSON object containing an array of forecast entries (`time` / `timestamp` and `irradiance` value in `W/m²`) so that the response can be easily extended with additional time slots in the future via configuration alone.

---

## 3. Non-Functional Requirements

- **NFR-01 (Language):** All source code, comments, configuration keys, and documentation must be written in English.
- **NFR-02 (Technology Stack):** .NET / C# (ASP.NET Core Web API).
- **NFR-03 (Output & Integration):** Expose functionality via REST API in the initial phase, with planned support for MQTT publishing in later phases.

---

## 4. Domain Model & Data Sources

### 4.1 Weather Provider
- **Open-Meteo Forecast API:** `https://api.open-meteo.com/v1/forecast`
- **Queried Variable:** Hourly `shortwave_radiation` (`W/m²`).

### 4.2 Expected JSON Response Example (`FR-01`)
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

---

## 5. Planned Future Requirements

- **Solar Production Calculation (kWh):** Convert irradiance forecasts into estimated PV production (`kWh`).
- **Overnight Battery Target SoC Calculation:** Calculate target battery `SoC %` using configurable daily/daytime consumption (`kWh`), battery capacity (`kWh`), and `MinSoC` / `MaxSoC` constraints.
- **MQTT Publication:** Publish forecast and target SoC results to an MQTT broker.
