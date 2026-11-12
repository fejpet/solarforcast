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

### FR-02: Next-Day Solar Production Estimation (kWh)
Estimate the total next-day photovoltaic (PV) energy generation in `kWh` based on the full-day hourly solar irradiance forecast and configured solar plant parameters.

- **FR-02.1 (Configurable PV System Parameters):** Read `SystemPeakPowerKwp` (installed peak capacity in `kWp`, default `5.0`) and `PerformanceRatio` (system efficiency factor between `0.0` and `1.0`, default `0.8`) from configuration (`appsettings.json`).
- **FR-02.2 (Production Formula):** Compute hourly PV energy yield as `(IrradianceWm2 / 1000.0) * SystemPeakPowerKwp * PerformanceRatio` and sum across all 24 hours of tomorrow to obtain `EstimatedProductionKwh`.

### FR-03: Overnight Battery Target SoC Calculation
Calculate the recommended overnight battery target State of Charge (`TargetSocPercent`) to pre-charge the battery during off-peak hours.

- **FR-03.1 (Configurable Battery & Consumption Parameters):** Read `ExpectedDailyConsumptionKwh` (default `10.0`), `BatteryCapacityKwh` (default `10.0`), `MinSocPercent` (default `20.0`), and `MaxSocPercent` (default `100.0`) from configuration (`appsettings.json`).
- **FR-03.2 (Energy-Balance Target SoC Formula):**
  - Compute expected energy deficit: `EnergyDeficitKwh = max(0, ExpectedDailyConsumptionKwh - EstimatedProductionKwh)`.
  - Convert the deficit into battery percentage points: `AdditionalSocPercent = (EnergyDeficitKwh / BatteryCapacityKwh) * 100.0`.
  - Compute `TargetSocPercent = clamp(MinSocPercent + AdditionalSocPercent, MinSocPercent, MaxSocPercent)`.
- **FR-03.3 (REST API Exposure):** Expose the estimated daily production (`kWh`) and calculated overnight `TargetSocPercent` via the REST API (both included in the tomorrow forecast response or via a dedicated battery/energy-plan endpoint).

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

### 4.2 Expected JSON Response Example (`FR-01` – `FR-03`)
```json
{
  "date": "2026-11-13",
  "unit": "W/m2",
  "estimatedProductionKwh": 6.45,
  "targetBatterySocPercent": 55.5,
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

- **FR-02 (Solar Production Calculation in kWh):** Estimate full-day next-day PV energy production (`kWh`) by summing the hourly solar irradiance across the day and scaling by the configured solar system peak power (`kWp`) and system efficiency (`PerformanceRatio`).
- **FR-03 (Overnight Battery Target SoC Calculation):** Calculate the required overnight battery target State of Charge (`SoC %`) using an energy-balance model (`ExpectedConsumptionKwh - ForecastedProductionKwh`), mapped to the battery's usable capacity (`BatteryCapacityKwh`) and clamped between `MinSocPercent` and `MaxSocPercent`.
- **FR-04 (MQTT Publication):** Publish forecast and target SoC results to an MQTT broker.
