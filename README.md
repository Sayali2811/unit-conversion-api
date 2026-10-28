# Unit Conversion API

An ASP.NET Core Web API that converts numeric values between units of measurement.
It supports **length**, **mass** and **temperature** out of the box and is structured so that adding hundreds of
units (or new categories) means adding data, not rewriting logic.

## Features

- `GET`/`POST` conversion endpoint, e.g. metres → feet, °C → °F, kilograms → pounds
- Unit lookup by **symbol, name or alias**, case-insensitive (`m`, `Metre`, `meters`, `°C`, `Celsius`)
- Discovery endpoint listing supported units, filterable by category
- Consistent error responses using RFC 9457 *Problem Details*
- Input validation, physical-limit validation (nothing below absolute zero), overflow protection
- OpenAPI document (Development), health check endpoint, unit + integration tests, CI workflow

| Category    | Units |
|-------------|-------|
| Length      | mm, cm, m, km, in, ft, yd, mi, nmi |
| Mass        | mg, g, kg, t, oz, lb, st |
| Temperature | C, F, K, R |

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) (latest stable; pinned via `global.json`)

## Run locally

```bash
git clone <repository-url>
cd <repository-folder>

dotnet restore
dotnet run --project src/UnitConversion.Api --launch-profile http
```

The API listens on `http://localhost:5080`. Try it:

```bash
curl "http://localhost:5080/api/v1/conversions?value=100&from=m&to=ft"
```

Prefer a REST client? Open `src/UnitConversion.Api/UnitConversion.Api.http` in Visual Studio, VS Code
(REST Client) or Rider and run the ready-made requests.

The OpenAPI document is served in Development at `http://localhost:5080/openapi/v1.json`.

### Run the tests

```bash
dotnet test
```

## API

### `GET /api/v1/conversions?value={number}&from={unit}&to={unit}`

```
GET /api/v1/conversions?value=100&from=C&to=F
```

```json
{
  "category": "Temperature",
  "value": 100,
  "from": "C",
  "to": "F",
  "result": 212
}
```

### `POST /api/v1/conversions`

```json
{ "value": 1, "from": "kg", "to": "lb" }
```

Returns the same response shape as the `GET` endpoint.

### `GET /api/v1/units[?category=length|mass|temperature]`

```json
[
  { "symbol": "ft", "name": "Foot", "category": "Length", "aliases": ["feet"] }
]
```

### `GET /health`

Returns `200 OK` when the service is running.

### Errors

Errors use `application/problem+json`.

| Situation | Status | Example `title` |
|-----------|--------|-----------------|
| Missing / malformed parameters | 400 | `One or more validation errors occurred.` |
| Units belong to different categories (`m` → `kg`) | 400 | `Incompatible units` |
| Value below absolute zero, or too large to convert | 400 | `Value out of range` |
| Unknown unit | 404 | `Unknown unit` |
| Unexpected failure | 500 | `An error occurred while processing your request.` |

```json
{
  "title": "Incompatible units",
  "status": 400,
  "detail": "Cannot convert Length unit 'm' to Mass unit 'kg'."
}
```

### Configuration

`src/UnitConversion.Api/appsettings.json`

| Key | Default | Description |
|-----|---------|-------------|
| `Conversion:DecimalPlaces` | `10` | Decimal places results are rounded to (0–28). Validated at startup. |

## Solution structure

```
src/
  UnitConversion.Domain          Unit model (UnitDefinition, UnitCategory) and domain exceptions. No dependencies.
  UnitConversion.Application     Conversion use case (IUnitConversionService) and the IUnitRepository abstraction.
  UnitConversion.Infrastructure  In-memory unit catalogue and repository (the hard-coded data lives here).
  UnitConversion.Api             HTTP layer: controllers, contracts, error handling, composition root.
tests/
  UnitConversion.Tests           Unit tests (domain, application, infrastructure) and API integration tests.
```

Dependencies point inwards: `Api → Application → Domain`, and `Infrastructure → Application → Domain`.
Only the API project wires the concrete implementations together.

## Design decisions

**Base-unit normalisation.** Every unit is described relative to one base unit per category (metre, kilogram,
kelvin). A conversion is `source → base → target`. With *N* units in a category this needs *N* definitions rather than
*N × N* conversion pairs, which is what makes "hundreds of units" tractable.

**One affine formula covers all current units.** Each unit stores `base = (value + offset) × factor`.
Length and mass only need a factor; Celsius, Fahrenheit and Rankine also need an offset. Temperature therefore needs
no special-case code. Applying the offset *before* scaling means that e.g. −459.67 °F maps to exactly 0 K.

**`decimal`, not `double`.** Conversion factors such as 0.3048 are exact in `decimal`, so results do not suffer binary
floating-point noise (`0.1 + 0.2`-style errors). Results are rounded to a configurable number of places and
trailing zeros are stripped.

**SOLID in practice.**
- *Single responsibility* – controllers only translate HTTP ⇄ application calls; the service only converts; the
  repository only stores/looks up units; the exception handler only maps errors to responses.
- *Open/closed* – new units are new entries in `UnitCatalog`; no conversion code changes. New categories need an enum
  value and, if they have a physical lower bound, one line in `UnitCategoryExtensions`.
- *Liskov / interface segregation* – small, focused interfaces (`IUnitRepository`, `IUnitConversionService`).
- *Dependency inversion* – the application layer depends on `IUnitRepository`, never on the in-memory implementation.
  Swapping in a database, cache or remote service is a one-line change in `Infrastructure/DependencyInjection.cs`.

**Domain failures are exceptions, mapped centrally.** Expected business errors (unknown unit, incompatible units,
out of range) derive from `UnitConversionException`. A single `IExceptionHandler` converts them to Problem Details, so
controllers contain no `try/catch` or status-code logic.

**Validation at the edge.** Data annotations and `[ApiController]` reject malformed input with a 400 before it reaches
the service. The service still guards its own invariants (blank identifiers, categories, physical limits).

**Versioned routes** (`/api/v1/...`) so breaking changes can be introduced side by side later.

## Trade-offs and limitations

- **Hard-coded, in-memory data.** As permitted for this version. The `IUnitRepository` seam is where a database would plug in.
  The repository validates identifier uniqueness at startup so catalogue mistakes fail fast.
- **Categories are an enum.** This gives compile-time safety for three categories, but adding a category requires a code
  change. If categories must become runtime data, `UnitCategory` would turn into a value object/string loaded from storage.
- **Only affine conversions are modelled.** Non-linear relationships (e.g. fuel economy, where L/100 km ↔ mpg is
  an inverse relation, or logarithmic scales) do not fit `(value + offset) × factor`. The intended extension is to put an
  `IConversionStrategy` per category/unit behind `UnitDefinition`; it was left out to avoid speculative abstraction.
- **Aliases are matched case-insensitively.** This is friendlier for callers, but means two units whose identifiers
  differ only by case (e.g. `Mm` vs `mm`) cannot coexist; startup validation reports such collisions.
- **No authentication, rate limiting or caching.** Out of scope for the exercise. The service is stateless and the
  catalogue is immutable, so the API scales horizontally and is easy to add these to.
- **Decimal range.** Values that overflow `decimal` return a 400 rather than silently losing precision.

## How to add a unit or category

- **New unit:** add one line to the relevant method in `src/UnitConversion.Infrastructure/Units/UnitCatalog.cs`
  (symbol, name, category, factor to the base unit, optional offset, aliases) and add a case to
  `UnitConversionServiceTests.ValidConversions`.
- **New category:** add a value to `UnitCategory`, add its units to the catalogue, and (if needed) a physical lower bound
  in `UnitCategoryExtensions.MinimumBaseValue`.

## Continuous integration

`.github/workflows/ci.yml` restores, builds and runs all tests on every push and pull request to `main`.
