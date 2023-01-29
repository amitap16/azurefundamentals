using AzureTangyFunc;
using AzureTangyFunc.Data;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace AzureTangyFunc
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("AzureSqlDatabase");
            builder.Services.AddDbContext<AzureTangyDbContext>(
                options => SqlServerDbContextOptionsExtensions.UseSqlServer(options, connectionString)
                );
        }
    }
}
