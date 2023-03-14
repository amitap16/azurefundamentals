using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureSpookyLogic.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Serialization;

namespace AzureSpookyLogic.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BlobServiceClient _blobServiceClient;
        static readonly HttpClient _httpClient = new HttpClient();


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
        public async Task<IActionResult> Index(SpookyRequest spookyRequest, IFormFile file)
        {
            spookyRequest.Id = Guid.NewGuid().ToString();
            var jsonContent = JsonConvert.SerializeObject(spookyRequest);
            using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage httpResponse = await _httpClient.PostAsync("https://prod-09.eastus.logic.azure.com:443/workflows/d5d17f19be424408be794692efd1bcc9/triggers/manual/paths/invoke?api-version=2016-10-01&sp=%2Ftriggers%2Fmanual%2Frun&sv=1.0&sig=LgvbNbZHyqyYP_VFDLx0bokt11PTeBmjyTbY7uI8QQY", content); ;
            }

            if (file != null)
            {
                var fileName = $"{spookyRequest.Id}.{Path.GetExtension(file.FileName)}";
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("logicappcontainer");
                BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

                BlobHttpHeaders httpHeaders = new BlobHttpHeaders()
                {
                    ContentType = file.ContentType
                };

                await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
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