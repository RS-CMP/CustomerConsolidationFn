# CustomerConsolidationFn Design Description

## Overview
CustomerConsolidationFn consolidates two NDJSON customer record files, normalizes country codes, and produces a deterministic, filtered output and summary. The solution is designed for clarity, maintainability, and moderate memory efficiency.

## Domain Model
- `CustomerRecord`: Raw input record.
- `NormalizedCustomerRecord`: Output record with original and ISO2 country codes.
- `CountryMapping`: Static dictionary for country name to ISO2 code.

## Service Logic
- Streams input files line-by-line.
- Filters records by `lastModifiedUtc >= cutoffDate`.
- Normalizes country names; unmapped countries are set to `null`.
- Tracks per-source record counts and unmapped country count.
- Buffers all output records for sorting by `Id` (deterministic output).
- Writes sorted records to `Data/consolidated.ndjson`.

## Deterministic Output
- All output records are sorted by `Id` to ensure identical results for identical input/configuration.
- This requires buffering all records before writing.

## Memory Efficiency
- Input is streamed, but output is buffered for sorting.
- Suitable for moderate data sizes; for very large datasets, external sorting or chunked processing is recommended.

## Azure Function Integration
- HTTP-triggered function parses `cutoffDate` and calls service logic.
- Returns summary as JSON.
- Handles errors gracefully with descriptive JSON responses.

## Limitations & Scalability
- Country mapping is limited; unmapped countries are counted.
- For very large datasets, consider external sorting or distributed processing.
- Input files must be present in the `Data` directory.

## Logging & Diagnostics
- Logs execution start/end, duration, and summary statistics.
- Use Visual Studio Diagnostic Tools for memory profiling.

## Extensibility
- Country mapping can be expanded.
- Service logic can be adapted for chunked/external sorting if needed.

