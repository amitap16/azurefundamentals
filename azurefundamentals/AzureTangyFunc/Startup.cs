using AzureTangyFunc;
using AzureTangyFunc.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: WebJobsStartup(typeof(Startup))]
namespace AzureTangyFunc
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureSqlDatabase");
            builder.Services.AddDbContext<AzureTangyDbContext>(
                options => options.UseSqlServer(connectionString));
        }
    }
}
