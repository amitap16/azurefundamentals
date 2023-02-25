using AzureTangyFunc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace AzureTangyFunc
{
    public static class OnSalesUploadWriteToQueue
    {
        private const string LOG_PREFIX = $"{nameof(OnSalesUploadWriteToQueue)}";

        [FunctionName("OnSalesUploadWriteToQueue")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [Queue("SalesRequestInBound", Connection = "AzureWebJobsStorage")] IAsyncCollector<SalesRequest> salesRequestQueue,
            ILogger log)
        {
            log.LogInformation($"{LOG_PREFIX} function: Sales request received.");

            string requestBody = await new StreamReader(req?.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(requestBody))
            {
                log.LogInformation($"{LOG_PREFIX} function: Sales request's HttpRequest Body is empty!");
                return new EmptyResult();
            }

            SalesRequest data = JsonConvert.DeserializeObject<SalesRequest>(requestBody);
            log.LogInformation($"Sales request data: {data}.");

            //Push data to queue
            await salesRequestQueue?.AddAsync(data);

            string responseMessage = $"Sales Request has been received for {data.Name}.";
            log.LogInformation($"{LOG_PREFIX} function: {nameof(responseMessage)} - {responseMessage}.");
            return new OkObjectResult(responseMessage);
        }
    }
}
