using Blogifier.Core.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;

namespace Blogifier
{
	public class Program
	{
		public static void Main(string[] args)
		{
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
				catch { }
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			 Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     if (context.HostingEnvironment.IsProduction())
                     {
                         var builtConfig = config.Build();

                         var azureServiceTokenProvider = new AzureServiceTokenProvider();

                         var keyVaultClient = new KeyVaultClient(
                             new KeyVaultClient.AuthenticationCallback(
                                 azureServiceTokenProvider.KeyVaultTokenCallback));

                         config.AddAzureKeyVault(
                             $"https://{builtConfig["KeyVaultName"]}.vault.azure.net/",
                             keyVaultClient,
                             new DefaultKeyVaultSecretManager());
                     }
                 })
                .ConfigureWebHostDefaults(webBuilder =>
				  {
					  webBuilder
					  .UseContentRoot(Directory.GetCurrentDirectory())
					  .UseIISIntegration()
					  .UseStartup<Startup>();
				  });
	}
}
