# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

TheSadRogue.Primitives is a .NET class library providing primitive data structures for 2D grid-based games/applications. It is published as a NuGet package (`TheSadRogue.Primitives`) and two optional integration packages for MonoGame and SFML.

## Solution Structure

- **`TheSadRogue.Primitives/`** — Core library. Multi-targeted: `net5.0;net6.0;net7.0;net8.0;net9.0;net10.0;netcoreapp3.1;netstandard2.1`. Root namespace: `SadRogue.Primitives`.
- **`TheSadRogue.Primitives.MonoGame/`** — Extension methods bridging core types to MonoGame equivalents.
- **`TheSadRogue.Primitives.SFML/`** — Extension methods bridging core types to SFML equivalents.
- **`TheSadRogue.Primitives.UnitTests/`** — Main xUnit test project targeting `net10.0`.
- **`TheSadRogue.Primitives.UnitTests.NonThreadSafe/`** — Tests that manipulate shared state (e.g., `YIncreasesUpwards`) and must run sequentially without parallelism, isolated from the main test project.
- **`TheSadRogue.Primitives.UnitTests.Shared/`** — Shared project (`.shproj`) containing common test code compiled into all unit test projects.
- **`TheSadRogue.Primitives.MonoGame.UnitTests/`** and **`TheSadRogue.Primitives.SFML.UnitTests/`** — Integration-level tests for the platform-specific packages.
- **`TheSadRogue.Primitives.PerformanceTests/`** — BenchmarkDotNet benchmarks.

## Common Commands

### Build
```bash
dotnet build
```

When checking for warnings or errors, always run `dotnet clean` first to avoid false results from stale incremental build artifacts:
```bash
dotnet clean && dotnet build
```

### Run all tests
```bash
dotnet test
```

### Run tests for a specific project
```bash
dotnet test TheSadRogue.Primitives.UnitTests/TheSadRogue.Primitives.UnitTests.csproj
dotnet test TheSadRogue.Primitives.UnitTests.NonThreadSafe/TheSadRogue.Primitives.UnitTests.NonThreadSafe.csproj
```

### Run a single test by name
```bash
dotnet test TheSadRogue.Primitives.UnitTests/TheSadRogue.Primitives.UnitTests.csproj --filter "FullyQualifiedName~TestClassName.TestMethodName"
```

### Run benchmarks
```bash
cd TheSadRogue.Primitives.PerformanceTests
dotnet run -c Release
```

### Pack NuGet packages
```bash
dotnet pack TheSadRogue.Primitives/TheSadRogue.Primitives.csproj -c Release
```
Packed `.nupkg` and `.snupkg` files are copied to a `nuget/` directory at the repo root.

## Architecture

### Core Primitives (`SadRogue.Primitives` namespace)

Key types in the core library:
- **`Point`** — Immutable 2D coordinate struct. Supports tuple conversion, deconstruction, and operators with `Direction`. Has a sentinel `Point.None = (int.MinValue, int.MinValue)`.
- **`Direction`** / **`AdjacencyRule`** — Cardinal/diagonal directions and neighbor enumeration rules. `Direction.YIncreasesUpwards` is a static flag that affects direction math — tests that toggle this must be in `UnitTests.NonThreadSafe`.
- **`Distance`** / **`Radius`** — Distance calculation strategies (Chebyshev, Euclidean, Manhattan) and corresponding radius shapes.
- **`Rectangle`** / **`BoundedRectangle`** — 2D rectangle operations.
- **`Area`** — An arbitrary set of grid positions.
- **`Lines`** / **`Shapes`** — Line drawing and shape generation algorithms.
- **`Color`** / **`Gradient`** / **`Palette`** — Color types.
- **`IDGenerator`** / **`IHasID`** / **`IHasLayer`** — Utilities for object identity and layering systems.

### GridViews (`SadRogue.Primitives.GridViews`)

An abstraction for treating any data source as a readable (or settable) 2D grid:
- **`IGridView<T>`** — Read-only interface with indexers by `Point` and `(x, y)`, as well as integer indexes.
- **`ISettableGridView<T>`** — Extends `IGridView<T>` with setters.
- **`GridViewBase<T>`** / **`SettableGridViewBase<T>`** — Abstract base classes to subclass for custom implementations.
- **`ArrayView<T>`** / **`ArrayView2D<T>`** — Back a grid view with a 1D or 2D array.
- **`LambdaGridView<T>`** / **`LambdaSettableGridView<T>`** — Back a grid view with a lambda/delegate.
- **`TranslationGridView<T,U>`** / **`LambdaTranslationGridView<T,U>`** — Wrap an existing grid view and transform values.
- **`Viewport<T>`** / **`SettableViewport<T>`** — A rectangular window into another grid view.
- **`DiffAwareGridView<T>`** — Tracks changes (diffs) to a settable grid view.

### SpatialMaps (`SadRogue.Primitives.SpatialMaps`)

Collections that map objects to 2D positions:
- **`SpatialMap<T>`** / **`AdvancedSpatialMap<T>`** — One item per position.
- **`MultiSpatialMap<T>`** / **`AdvancedMultiSpatialMap<T>`** — Multiple items per position.
- **`LayeredSpatialMap<T>`** / **`AdvancedLayeredSpatialMap<T>`** — Items organized by layers, with a `LayerMasker` for bitfield-based layer selection.
- **AutoSync variants** (`AutoSyncSpatialMap<T>`, etc.) — Automatically keep position in the map synchronized when an item's `IPositionable.Position` property changes.

### Serialization (`SadRogue.Primitives.SerializedTypes`)

Parallel types in `SerializedTypes/` that mirror the core types but are designed for JSON/binary serialization, with implicit conversions to/from the main types. Tested via `DataContractTests`, `BinaryTests`, and `ImplicitConversionTests` in the unit tests.

### Performance Conventions

Many hot-path types use custom struct enumerators (e.g., `SpatialMapItemsAtEnumerator<T>`, `RectanglePositionsEnumerator`) instead of `IEnumerable<T>` with `yield return` to avoid allocation and boxing in `foreach` loops. These structs implement both `IEnumerable<T>` and `IEnumerator<T>` but incur boxing overhead when passed as `IEnumerable<T>`.

## Documentation Style

All public and protected members require XML doc comments. Conventions for specific cases:

- **Implicit conversion operators** (throughout `SerializedTypes/`) intentionally use empty self-closing `<param>` and `<returns>` tags (e.g., `<param name="rect"/>`, `<returns/>`). The summary already states what is being converted to/from, and the type signatures make the parameter and return self-evident. Do not fill these in with boilerplate like "The value to convert." — that is considered noise, not documentation.
- **Math/equality operators** similarly leave `<param>` tags empty when operand names are self-documenting (e.g., `c`, `i`, `other`).
- **`GetHashCode`, `ToString`, `GetEnumerator`** return tags may be left empty; the return values are self-evident.
- **`<typeparam name="T"/>`** may be left as an empty self-closing tag in extension methods where the type parameter is obvious from context.

## Code Style

Enforced by `.editorconfig`:
- 4-space indentation, CRLF line endings.
- Private/internal fields: `_camelCase`. Static private fields: `s_camelCase`. Constants: `PascalCase`.
- No `var` except when type is apparent.
- `using` directives outside namespace, System directives first.
- All public APIs annotated with JetBrains `[PublicAPI]` attribute (from `JetBrains.Annotations`, private assets).
- C# language version fixed at 8.0 across all projects.
- Nullable reference types enabled (`<Nullable>enable</Nullable>`).