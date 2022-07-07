using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BF2TV.Backend.Functions.DiscordApi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Refit;

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
        [OpenApiOperation(operationId: "Run")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "text/plain",
            bodyType: typeof(string))]
        public async Task<IActionResult> Messages(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = $"{nameof(Discord)}/{nameof(Messages)}")]
            HttpRequest req)
        {
            _logger.LogInformation("Azure Function entry point of {Method} was triggered", nameof(Messages));

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var token = config["DiscordAuth"];
            if (token == null)
                return new OkObjectResult("Couldn't resolve secret");

            var channelId = "991744806462160896";
            var authenticationPhrase = $"Bot {token}";

            var discordApi = RestService.For<IDiscordApi>("https://discord.com/api/v10");
            var messages = await discordApi.GetMessagesForChannelId(channelId, authenticationPhrase);

            return new OkObjectResult(messages);
        }
    }
}