using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CustomerConsolidationFn.Functions;

public class ConsolidateFunction
{
    private readonly ILogger<ConsolidateFunction> _logger;

    public ConsolidateFunction(ILogger<ConsolidateFunction> logger)
    {
        _logger = logger;
    }

    [Function("ConsolidateFunction")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}