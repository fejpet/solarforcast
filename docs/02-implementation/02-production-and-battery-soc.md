# Implementation Plan: 02 — Next-Day PV Production (kWh) & Overnight Battery Target SoC Calculation

- **Status:** Completed
- **Related Requirements:** `FR-02` (`FR-02.1`, `FR-02.2`), `FR-03` (`FR-03.1`, `FR-03.2`, `FR-03.3`) (see `docs/01-requirement/requirement.md`)

---

## 1. Context & Goal

Extend the **SolarForecast** service and REST API so that, in addition to returning the configured hourly solar irradiance slots (`W/m²`), it calculates:
1. **Tomorrow's Estimated PV Energy Production (`kWh`)** by summing the full-day hourly solar irradiance (`W/m²`) and applying `SystemPeakPowerKwp` and `PerformanceRatio`.
2. **Overnight Battery Target State of Charge (`TargetSocPercent`)** using an energy-balance model (`ExpectedDailyConsumptionKwh - EstimatedProductionKwh`) mapped onto `BatteryCapacityKwh` between `MinSocPercent` and `MaxSocPercent`.

---

## 2. Technical Design

### 2.1 Configuration Extensions (`SolarForecastOptions`)
Add the following configurable properties to `SolarForecastOptions` and `appsettings.json`:
- `SystemPeakPowerKwp` (`double`, default `5.0`)
- `PerformanceRatio` (`double`, default `0.8`)
- `ExpectedDailyConsumptionKwh` (`double`, default `10.0`)
- `BatteryCapacityKwh` (`double`, default `10.0`)
- `MinSocPercent` (`double`, default `20.0`)
- `MaxSocPercent` (`double`, default `100.0`)

### 2.2 Calculation Logic (`SolarForecastService`)
- **Estimated Production (`kWh`):**
  $$\text{EstimatedProductionKwh} = \sum_{h=0}^{23} \left(\frac{\text{Irradiance}_h}{1000.0}\right) \times \text{SystemPeakPowerKwp} \times \text{PerformanceRatio}$$
- **Energy Deficit (`kWh`):**
  $$\text{EnergyDeficitKwh} = \max(0, \text{ExpectedDailyConsumptionKwh} - \text{EstimatedProductionKwh})$$
- **Target Battery SoC (`%`):**
  $$\text{TargetSocPercent} = \text{ clamp}\left(\text{MinSocPercent} + \frac{\text{EnergyDeficitKwh}}{\text{BatteryCapacityKwh}} \times 100.0, \text{MinSocPercent}, \text{MaxSocPercent}\right)$$

### 2.3 API Endpoints & Models
- Extend `TomorrowIrradianceResponse` or add `GET /api/forecast/tomorrow/battery-plan` (and include `estimatedProductionKwh` and `targetBatterySocPercent` in the summary) so clients can retrieve both the hourly irradiance array and the calculated energy/SoC plan.

---

## 3. Task Breakdown

- [x] **Task 1:** Extend `SolarForecastOptions` and `appsettings.json` with PV system and battery parameters.
- [x] **Task 2:** Implement PV production (`kWh`) and overnight battery Target SoC (`%`) calculation in `SolarForecastService`.
- [x] **Task 3:** Expose the production and battery target SoC fields via the REST API.
- [x] **Task 4:** Add unit tests covering sunny day (high production -> `MinSocPercent`), cloudy day (low production -> `MaxSocPercent`), and partial deficit scenarios.
- [x] **Task 5:** Update `README.md` with the new configuration parameters and response fields.

---

## 4. Verification Plan

- Run `dotnet test SolarForecast.slnx` and verify all existing and new unit tests pass.
