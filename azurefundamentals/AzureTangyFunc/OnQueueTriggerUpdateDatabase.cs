//using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace AzureTangyFunc
{
    public class OnQueueTriggerUpdateDatabase
    {
        /* private readonly AzureTangyDbContext _dbContext;

         public OnQueueTriggerUpdateDatabase(AzureTangyDbContext dbContext)
         {
             _dbContext = dbContext;
         }

        [FunctionName("OnQueueTriggerUpdateDatabase")]
        public void Run([QueueTrigger("SalesRequestInBound", Connection = "AzureWebJobsStorage")] SalesRequest myQueueItem,
            ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
            myQueueItem.Status = "Submitted";
            //_dbContext.SalesRequests.Add(myQueueItem);
            //_dbContext.SaveChanges();
        }*/
    }
}