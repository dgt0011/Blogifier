using Blogifier.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
namespace Blogifier
{
    public class Program
    {
        private static ILogger Logger;

        public static void Main(string[] args)
        {
            var LoggerF = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Blogifier", LogLevel.Trace)
                .AddConsole().AddAzureWebAppDiagnostics()
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning);
            });
            Logger = LoggerF.CreateLogger<Program>();

            var host = CreateHostBuilder(args).Build();            

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<AppDbContext>();

                try
                {
                    if (dbContext.Database.GetPendingMigrations().Any())
                        dbContext.Database.Migrate();
                }
                catch (Exception ex) {
                    Logger.LogError(ex.ToString());
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
             Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     if (context.HostingEnvironment.IsProduction())
                     {
                     }
                 })
            //.ConfigureLogging(logging => logging.AddAzureWebAppDiagnostics())
            //.ConfigureServices(serviceCollection => serviceCollection
            //    .Configure<AzureFileLoggerOptions>(options =>
            //    {
            //        options.FileName = "azure-diagnostics-";
            //        options.FileSizeLimit = 50 * 1024;
            //        options.RetainedFileCountLimit = 5;
            //    }).Configure<AzureBlobLoggerOptions>(options =>
            //    {
            //        options.BlobName = "log.txt";
            //    })
             //   )
                .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder
                      .UseContentRoot(Directory.GetCurrentDirectory())
                      .UseIISIntegration()
                      .UseStartup<Startup>();
                  });
    }
}
