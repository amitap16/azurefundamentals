using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AzureTangyFunc
{
    public class OnImageProcessedEvery5MinutesUpdateStatusToCompleted
    {
        private readonly AzureTangyDbContext _dbContext;

        public OnImageProcessedEvery5MinutesUpdateStatusToCompleted(AzureTangyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("OnImageProcessedEvery5MinutesUpdateStatusToCompleted")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            IEnumerable<SalesRequest> salesRequestsFromDB = _dbContext.SalesRequests.Where(x => x.Status == "Image Processed");
            foreach (SalesRequest salesRequest in salesRequestsFromDB)
            {
                salesRequest.Status = "Completed";
            }

            _dbContext.UpdateRange(salesRequestsFromDB);
            _dbContext.SaveChanges();
        }
    }
}
