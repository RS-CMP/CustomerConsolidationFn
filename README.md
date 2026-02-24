# CustomerConsolidationFn

## Run Instructions

1. **Requirements:**
   - .NET 8 SDK
   - Azure Functions Core Tools (for local function testing)
   - Input files: `Data/output1.ndjson` and `Data/output2.ndjson` (NDJSON format)

2. **Build and Run Locally:**
   - Open the solution in Visual Studio.
   - Build the project.
   - Start the Azure Functions host (F5 or `func start`).

3. **Trigger the Function:**
   - Send a POST request to `http://localhost:<port>/api/ConsolidateFunction` with a JSON body:
     ```json
     { "cutoffDate": "2024-05-01" }
     ```
   - Use PowerShell:
     ```powershell
     Invoke-RestMethod -Uri "http://localhost:<port>/api/ConsolidateFunction" -Method POST -Headers @{ "Content-Type" = "application/json" } -Body '{ "cutoffDate": "2024-05-01" }'
     ```

4. **Output:**
   - Consolidated NDJSON file is written to `Data/consolidated.ndjson`.
   - Function returns a summary of records processed.

## Assumptions

- Input files are NDJSON, each line is a JSON customer record.
- All records passing the cutoff date are included in the output, regardless of country mapping.
- Unmapped countries have `CountryIso2` set to `null`.
- Output is deterministic: records are sorted by `Id`.
- Memory usage is efficient for moderate data sizes; for very large datasets, external sorting may be required.
- Country mapping is limited; unmapped countries are counted in the summary.

## Sample Error Handling
- If `cutoffDate` is missing or invalid, the function returns a JSON error with HTTP 400.

## Project Structure
- Domain models: `/Domain/`
- Service logic: `/Services/`
- Azure Function: `/Functions/`
- Data files: `/Data/`
- Documentation: `/Docs/`
