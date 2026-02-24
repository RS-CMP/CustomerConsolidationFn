using CustomerConsolidationFn.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CustomerConsolidationFn.Functions;

public class ConsolidateFunction
{
    private readonly ILogger<ConsolidateFunction> _logger;

    public ConsolidateFunction(ILogger<ConsolidateFunction> logger)
    {
        _logger = logger;
    }

    [Function("ConsolidateFunction")]
    public async Task<IActionResult> Run([
        HttpTrigger(AuthorizationLevel.Anonymous, "post")
    ] HttpRequest req)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("ConsolidateFunction HTTP trigger invoked at {StartTime} UTC.", startTime);

        // Parse cutoffDate from query or body
        string? cutoffDateStr = req.Query["cutoffDate"];
        if (string.IsNullOrWhiteSpace(cutoffDateStr) && req.Body != null)
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("cutoffDate", out var cutoffProp))
                    {
                        cutoffDateStr = cutoffProp.GetString();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse request body as JSON while extracting cutoffDate.");
                }
            }
        }

        if (!DateTime.TryParse(cutoffDateStr, out var cutoffDate))
        {
            // Return a JSON error object for clarity
            return new BadRequestObjectResult(new {
                error = "cutoffDate is required in query or body and must be a valid date (e.g., 2024-05-01 or 2024-05-01T00:00:00)."
            });
        }

        // Resolve Data directory relative to project root so function works regardless of working directory
        // This is necessary because Azure Functions and .NET apps often run from bin/Debug/net8.0 or similar
        string ResolveDataPath(string fileName)
        {
            // Traverse up from base directory to find the project root containing the Data folder
            var baseDir = AppContext.BaseDirectory;
            var dir = new DirectoryInfo(baseDir);
            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "Data")))
            {
                dir = dir.Parent;
            }
            if (dir == null)
                throw new DirectoryNotFoundException("Could not find 'Data' directory relative to application base directory.");
            return Path.Combine(dir.FullName, "Data", fileName);
        }

        var inputPath1 = ResolveDataPath("output1.ndjson");
        var inputPath2 = ResolveDataPath("output2.ndjson");
        var outputPath = ResolveDataPath("consolidated.ndjson");

        // Call service logic
        var service = new CustomerConsolidationService();
        var summary = await service.ConsolidateAsync(inputPath1, inputPath2, outputPath, cutoffDate);

        var endTime = DateTime.UtcNow;
        var duration = endTime - startTime;

        _logger.LogInformation(
            "\nConsolidation completed at {EndTime} UTC.\nDuration: {Duration}.",
            endTime, duration);
        _logger.LogInformation(
            "\nRecords written: {RecordsWritten}\nUnmapped countries: {UnmappedCountryCount}\nRecords read from Source1: {Source1}\nRecords read from Source2: {Source2}",
            summary.RecordsWritten, summary.UnmappedCountryCount, summary.RecordsReadFromSource1, summary.RecordsReadFromSource2);

        // Return summary as JSON
        return new JsonResult(summary);
    }
}