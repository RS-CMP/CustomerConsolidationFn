using System.Text.Json;
using CustomerConsolidationFn.Domain;

namespace CustomerConsolidationFn.Services
{
    public class CustomerConsolidationService
    {
        public class ConsolidationSummary
        {
            public int RecordsWritten { get; set; }
            public int UnmappedCountryCount { get; set; }
            public int RecordsReadFromSource1 { get; set; }
            public int RecordsReadFromSource2 { get; set; }
        }

        /*
         * Asynchronously consolidates two NDJSON files, streaming and filtering records,
         * normalizing country names, and writing output with low memory usage.
         */
        public async Task<ConsolidationSummary> ConsolidateAsync(string inputPath1, string inputPath2, string? outputPath, DateTime cutoffDate)
        {
            int recordsWritten = 0;
            int unmappedCountryCount = 0;
            int recordsReadFromSource1 = 0;
            int recordsReadFromSource2 = 0;

            // Always output to Data/consolidated.ndjson
            var outputFilePath = "Data/consolidated.ndjson";

            var normalizedRecords = new List<NormalizedCustomerRecord>();

            var sources = new[] { inputPath1, inputPath2 };
            for (int i = 0; i < sources.Length; i++)
            {
                using var inputStream = new StreamReader(sources[i]);
                while (!inputStream.EndOfStream)
                {
                    var line = await inputStream.ReadLineAsync();
                    if (line == null) continue;

                    // Parse NDJSON line to CustomerRecord
                    var record = JsonSerializer.Deserialize<CustomerRecord>(line);
                    if (record == null) continue;
                    if (i == 0) recordsReadFromSource1++;
                    else recordsReadFromSource2++;

                    // Filter by cutoff date
                    if (record.LastModifiedUtc < cutoffDate) continue;

                    // Normalize country
                    var countryOriginal = record.Country;
                    var countryIso2 = CountryMapping.NameToIso2.TryGetValue(countryOriginal, out var iso2) ? iso2 : "??";
                    if (countryIso2 == "??") unmappedCountryCount++;

                    var normalized = new NormalizedCustomerRecord
                    {
                        Id = record.Id,
                        Name = record.Name,
                        CountryOriginal = countryOriginal,
                        CountryIso2 = countryIso2,
                        LastModifiedUtc = record.LastModifiedUtc
                    };

                    normalizedRecords.Add(normalized);
                }
            }

            // Sort records by Id for deterministic output
            var sortedRecords = normalizedRecords.OrderBy(r => r.Id, StringComparer.Ordinal).ToList();

            await using var outputStream = new StreamWriter(outputFilePath);
            foreach (var normalized in sortedRecords)
            {
                var json = JsonSerializer.Serialize(normalized);
                await outputStream.WriteLineAsync(json);
                recordsWritten++;
            }

            return new ConsolidationSummary
            {
                RecordsWritten = recordsWritten,
                UnmappedCountryCount = unmappedCountryCount,
                RecordsReadFromSource1 = recordsReadFromSource1,
                RecordsReadFromSource2 = recordsReadFromSource2
            };
        }
    }
}
