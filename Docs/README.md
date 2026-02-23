# Customer Data Consolidation Function

This project is an Azure Functions solution for an integration coding challenge. Its purpose is to consolidate customer records from two independent NDJSON source files, simulating Salesforce exports, into a single normalized, filtered dataset with summary statistics.

---

## Project Structure

- **Functions/**: Contains the Azure Function HTTP trigger (`ConsolidateFunction.cs`), the entry point for processing.
- **Services/**: Holds business logic for streaming reads, transformation, filtering, and consolidation.
- **Domain/**: Models and static mapping data for customer and country information.
- **Data/**: Stores input NDJSON files (`output1.ndjson`, `output2.ndjson`), the output file (`consolidated.ndjson`), and utility scripts (such as the sample data generator).
- **GenerateCustomerData.cs**: A standalone script (excluded from the main project), used for generating sample customer data as NDJSON files for testing and development.
- **README.md**: This documentation.

---

## Function Overview

- **Input**: Two NDJSON files, each representing customer record exports from separate Salesforce instances.
- **Processing**:
    - Streams both files for memory efficiency (does not load all data at once).
    - Filters records by a provided cutoff date (`lastModifiedUtc >= cutoffDate`).
    - Normalizes country names to ISO 2-letter codes, preserving the original and marking unmapped countries.
    - Consolidates records into a single NDJSON output.
    - Outputs summary statistics (records read per source, records written, unmapped countries).
- **Output**: One NDJSON file with consolidated records, plus a summary returned via HTTP response.

---

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

### Script Adaptations for dotnet-script

When adapting this script for use with dotnet-script, two changes were necessary:

1. **Use `Args` Instead of `args`:**
    - Standard console apps use `args` for command line arguments.
    - dotnet-script expects `Args` (capital A), so all references were updated accordingly.

2. **Convert `Args` to an Array for Indexing and Length:**
    - dotnet-script's `Args` is an `IList<string>`, not an array, so it was converted to an array with `Args.ToArray()` for compatibility with methods like `Array.IndexOf()` and `.Length`.

**Adapted argument handler:**
```csharp
int GetArg(string name, int defaultValue)
{
    var argsArr = Args.ToArray();
    var index = Array.IndexOf(argsArr, name);
    if (index >= 0 && index < argsArr.Length - 1)
        return int.Parse(argsArr[index + 1]);
    return defaultValue;
}
```

---

## Assignment Guidelines

- No tests or cloud resources required.
- All processing and file IO is local and memory-efficient.
- Project is organized for clarity and maintainability.

---

## Summary

This solution demonstrates data ingestion, normalization, consolidation, and summary reporting for large customer datasets using Azure Functions, following robust design practices. The standalone data generator enables flexible testing and development.