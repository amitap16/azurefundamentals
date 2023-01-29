using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureTangyFunc
{
    public class OnQueueTriggerUpdateDatabase
    {
        [FunctionName("OnQueueTriggerUpdateDatabase")]
        public void Run([QueueTrigger("SalesRequestInBound", Connection = "AzureWebJobsStorage")]string myQueueItem, 
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}