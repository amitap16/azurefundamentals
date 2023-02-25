using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace AzureTangyFunc
{
    public class OnResizeImageUpdateDatabase
    {
        private readonly AzureTangyDbContext _dbContext;

        public OnResizeImageUpdateDatabase(AzureTangyDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        [FunctionName("OnResizeImageUpdateDatabase")]
        public void Run([BlobTrigger("functionsalesrep-sm/{name}", Connection = "AzureWebJobsStorage")] Stream myBlob,
            string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string id = Path.GetFileNameWithoutExtension(name);
            SalesRequest salesRequestFromDB = _dbContext.SalesRequests.FirstOrDefault(x => x.Id == id);
            if (salesRequestFromDB != null)
            {
                salesRequestFromDB.Status = "Image Processed";
                _dbContext.SalesRequests.Update(salesRequestFromDB);
                _dbContext.SaveChanges();
            }
        }
    }
}
