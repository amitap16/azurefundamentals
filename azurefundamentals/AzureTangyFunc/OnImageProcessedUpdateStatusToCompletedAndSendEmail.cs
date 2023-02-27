using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureTangyFunc
{
    public class OnImageProcessedUpdateStatusToCompletedAndSendEmail
    {
        private readonly AzureTangyDbContext _dbContext;

        public OnImageProcessedUpdateStatusToCompletedAndSendEmail(AzureTangyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("OnImageProcessedUpdateStatusToCompletedAndSendEmail")]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
            [SendGrid(ApiKey = "SendgridForAzureFundamentals")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            int counter = 0;
            #region Update Status in DB
            IEnumerable<SalesRequest> salesRequestsFromDB = _dbContext.SalesRequests.Where(x => x.Status == "Image Processed");
            foreach (SalesRequest salesRequest in salesRequestsFromDB)
            {
                salesRequest.Status = "Completed";
                counter++;
            }

            _dbContext.UpdateRange(salesRequestsFromDB);
            _dbContext.SaveChanges();
            #endregion

            #region Send Email
            if (counter > 0)
            {
                var message = new SendGridMessage();
                message.AddTo("Edu4kid18@outlook.com");
                message.AddContent("text/html", $"Processing completed for {counter} records");
                message.SetFrom(new EmailAddress("aanchu10@outlook.com"));
                message.SetSubject("Azure Tangy Processing successful");
                await messageCollector.AddAsync(message);
            }
            #endregion
        }
    }
}
