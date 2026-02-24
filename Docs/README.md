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

---

# Customer Data Consolidation Function

This project is an Azure Functions solution for an integration coding challenge. Its purpose is to consolidate customer records from two independent NDJSON source files, simulating Salesforce exports, into a single normalized, filtered dataset with summary statistics.

## Function Overview

- **Input**: Two NDJSON files, each representing customer record exports from separate Salesforce instances.
- **Processing**:
    - Streams both files for memory efficiency (does not load all data at once).
    - Filters records by a provided cutoff date (`lastModifiedUtc >= cutoffDate`).
    - Normalizes country names to ISO 2-letter codes, preserving the original and marking unmapped countries.
    - Consolidates records into a single NDJSON output.
    - Outputs summary statistics (records read per source, records written, unmapped countries).
- **Output**: One NDJSON file with consolidated records, plus a summary returned via HTTP response.

## Sample Data Generator

The `/Data/GenerateCustomerData.cs` script is provided for generating sample NDJSON files (`output1.ndjson` and `output2.ndjson`).
> **Note:** This script is not part of the Azure Function project and is excluded from the build, so it doesn't conflict with the main codebase. It may be run independently to produce fresh demo data.

### Running the Data Generator

**Prerequisites:**
- [.NET SDK](https://dotnet.microsoft.com/download) installed.
- [dotnet-script](https://github.com/dotnet-script/dotnet-script) tool installed.

Install dotnet-script globally (if you haven't already):
```bash
dotnet tool install -g dotnet-script
```

**To run the script:**

1. Navigate to your project’s `/Data/` directory.
2. Run:
   ```bash
   dotnet-script GenerateCustomerData.cs -- --count 300000
   ```
   The `--count` argument controls how many records to generate per source (adjust as needed).
3. After running, `output1.ndjson` and `output2.ndjson` will be generated in the `/Data/` folder.
