using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureFunctionTangyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureFunctionTangyWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        static readonly HttpClient _httpClient = new();

        public HomeController(ILogger<HomeController> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(SalesRequest salesRequest, IFormFile file)
        {
            salesRequest.Id = Guid.NewGuid().ToString();

            using (var content = new StringContent(
                JsonConvert.SerializeObject(salesRequest)
                , System.Text.Encoding.UTF8, "application/json"))
            {
                //call azure function and pass the content
                HttpResponseMessage httpResponse
                    = await _httpClient.PostAsync("http://localhost:7192/api/OnSalesUploadWriteToQueue", content);

                string returnResult = httpResponse.Content.ReadAsStringAsync().Result;
            }
            if (file != null)
            {
                string fileName = salesRequest.Id + Path.GetExtension(file.FileName);
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("functionsalesrep");
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                var httpHeaders = new BlobHttpHeaders() { ContentType = file.ContentType };

                await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

                return View();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}