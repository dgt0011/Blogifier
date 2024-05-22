using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Blogifier.Core.Data;
using Blogifier.Core.Providers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blogifier.Core.Extensions
{
    public static class ServiceCollectionExtensions
	{
      public static IServiceCollection AddBlogDatabase(this IServiceCollection services, IConfiguration configuration)
      {
			var section = configuration.GetSection("Blogifier");
			var conn = section.GetValue<string>("ConnString");

			if (section.GetValue<string>("DbProvider") == "SQLite")
				services.AddDbContext<AppDbContext>(o => o.UseSqlite(conn));

            if (section.GetValue<string>("DbProvider") == "SqlServer")
            {
                var builder = new SqlConnectionStringBuilder(section.GetValue<string>("ConnString"))
                {           
                    UserID = configuration["Blog-DbUserId"],
                    Password = configuration["Blog-DbPassword"]
                };

                services.AddDbContext<AppDbContext>(o => o.UseSqlServer(builder.ConnectionString));
            }

            if (section.GetValue<string>("DbProvider") == "Postgres")
            {
                conn = GetSecureAwsConnectionString(configuration);
                services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));
            }

			//TODO: this is not tested
			if (section.GetValue<string>("DbProvider") == "MySql")
			{
				//services.AddDbContextPool<AppDbContext>(
				//	dbContextOptions => dbContextOptions.UseMySql(
				//		section.GetValue<string>("ConnString"),
				//		new MySqlServerVersion(new Version(8, 0, 21)),
				//		mySqlOptions => mySqlOptions.HasCharSet("utf8mb4", DelegationModes.ApplyToAll) //CharSetBehavior(CharSetBehavior.NeverAppend)
				//	)
				//);
			}
			services.AddDatabaseDeveloperPageExceptionFilter();
			return services;
      }

		public static IServiceCollection AddBlogProviders(this IServiceCollection services)
		{
			services.AddScoped<IAuthorProvider, AuthorProvider>();
			services.AddScoped<IBlogProvider, BlogProvider>();
			services.AddScoped<IPostProvider, PostProvider>();
			services.AddScoped<IStorageProvider, StorageProvider>();
			services.AddScoped<IFeedProvider, FeedProvider>();
			services.AddScoped<ICategoryProvider, CategoryProvider>();
			services.AddScoped<IAnalyticsProvider, AnalyticsProvider>();
			services.AddScoped<INewsletterProvider, NewsletterProvider>();
			services.AddScoped<IEmailProvider, MailKitProvider>();
			services.AddScoped<IThemeProvider, ThemeProvider>();
			services.AddScoped<ISyndicationProvider, SyndicationProvider>();
			services.AddScoped<IAboutProvider, AboutProvider>();

			return services;
		}

        private static string GetSecureAwsConnectionString(IConfiguration configuration)
        {
            var section = configuration.GetSection("DBCredentials");
            var awsSecretName = section.GetValue<string>("AWS-SecretName");

            if (awsSecretName != null)
            {
                var secretsManager = new AmazonSecretsManagerClient();  // the assumption is that this is running in a context that permits access to the required AWS Secrets MAnager secret

                var result = secretsManager.GetSecretValueAsync(
                    new GetSecretValueRequest { SecretId = awsSecretName }
                ).Result;

                if (result != null)
                {
                    try
                    {
                        var secretJObject = JObject.Parse(result.SecretString);

                        if (secretJObject["host"] != null &&
                            secretJObject["dbInstanceIdentifier"] != null &&
                            secretJObject["port"] != null &&
                            secretJObject["dbname"] != null &&
                            secretJObject["username"] != null &&
                            secretJObject["password"] != null)
                        {
                            var host = secretJObject["host"]!.ToString();
                            var port = secretJObject["port"]!.ToString();
                            var userName = secretJObject["username"]!.ToString();
                            var passWord = secretJObject["password"]!.ToString();
                            var database = secretJObject["dbname"]!.ToString();

                            return $"Host={host};Port={port};Username={userName};Password={passWord};Database={database};Timeout=14;Pooling=true;MinPoolSize=100;MaxPoolSize=200;";
                        }

                        //TODO: Log this somewhere
                        return string.Empty;

                    }
                    catch (JsonReaderException)
                    {
                        //TODO: Log this somewhere
                        return string.Empty;
                    }
                }

                //TODO: Log this somewhere
                return string.Empty;
            }

            //TODO: Log this somewhere
            return string.Empty;
        }
    }
}
