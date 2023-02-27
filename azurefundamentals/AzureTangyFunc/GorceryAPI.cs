using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace AzureTangyFunc
{
    public class GorceryAPI
    {
        private readonly AzureTangyDbContext _dbContext;

        public GorceryAPI(AzureTangyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [FunctionName("CreateGorcery")]
        public async Task<IActionResult> CreateGorcery(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "GroceryList")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating Grocery List Item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            GroceryItem groceryItem = new() { Name = data.Name };

            _dbContext.GroceryItems.Add(groceryItem);
            _dbContext.SaveChanges();

            return new OkObjectResult(groceryItem);
        }

        [FunctionName("GetGorcery")]
        public async Task<IActionResult> GetGorcery(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting all Grocery List Items.");

            return new OkObjectResult(await _dbContext.GroceryItems.ToListAsync());
        }

        [FunctionName("GetGorceryById")]
        public async Task<IActionResult> GetGorceryById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Getting Grocery List Item by Id.");

            GroceryItem item = await _dbContext.GroceryItems.FirstOrDefaultAsync(g => g.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(item);
        }

        [FunctionName("UpdateGorcery")]
        public async Task<IActionResult> UpdateGorcery(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "GroceryList/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Updating Grocery List Item.");
            GroceryItem groceryItem = await _dbContext.GroceryItems.FirstOrDefaultAsync(g => g.Id == id);
            if (groceryItem == null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);
            if (!string.IsNullOrEmpty(data.Name))
            {
                groceryItem.Name = data.Name;
                _dbContext.GroceryItems.Update(groceryItem);
                _dbContext.SaveChanges();
            }

            return new OkObjectResult(groceryItem);
        }

        [FunctionName("DeleteGorceryById")]
        public async Task<IActionResult> DeleteGorceryById(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "GroceryList/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation("Delete Grocery List Item by Id.");

            GroceryItem item = await _dbContext.GroceryItems.FirstOrDefaultAsync(g => g.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }

            _dbContext.GroceryItems.Remove(item);
            _dbContext.SaveChanges();

            return new OkResult();
        }
    }
}
