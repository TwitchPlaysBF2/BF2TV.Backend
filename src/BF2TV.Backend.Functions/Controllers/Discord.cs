using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace BF2TV.Backend.Functions.Controllers
{
    public class Discord
    {
        private readonly ILogger<Discord> _logger;

        public Discord(ILogger<Discord> log)
        {
            _logger = log;
        }

        [FunctionName(nameof(Messages))]

        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]

        [OpenApiSecurity(
            "function_key",
            SecuritySchemeType.ApiKey,
            Name = "code",
            In = OpenApiSecurityLocationType.Query)]

        [OpenApiParameter(
            name: "name",
            In = ParameterLocation.Query,
            Required = true,
            Type = typeof(string),
            Description = "The **Name** parameter")]

        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "text/plain",
            bodyType: typeof(string),
            Description = "The OK response")]

        public async Task<IActionResult> Messages(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{nameof(Discord)}/{nameof(Messages)}")]
            HttpRequest req)
        {
            _logger.LogInformation("Azure Function entry point of {Method} was triggered", nameof(Messages));

            string name = req.Query["name"];

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var responseMessage = string.IsNullOrEmpty(name)
                ? "Missing query parameter `name`."
                : $"Hello, {name}.";

            return new OkObjectResult(responseMessage);
        }
    }
}