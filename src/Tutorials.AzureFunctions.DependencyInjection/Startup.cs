using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tutorials.AzureFunctions.DependencyInjection;
using Tutorials.AzureFunctions.DependencyInjection.Core;
using Tutorials.AzureFunctions.DependencyInjection.Extensions;

[assembly: WebJobsStartup(typeof(Startup))]
namespace Tutorials.AzureFunctions.DependencyInjection
{
    public class Startup : IWebJobsStartup
    {
        public static bool IsDevelopment = !Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("PRODUCTION", StringComparison.OrdinalIgnoreCase);


        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton((s) => GetConfiguration(s));
            builder.Services.AddSingleton<ISingletonLifetimeManagement, SingletonLifetimeManagement>();
            builder.Services.AddScoped<IScopedLifetimeManagement, ScopedLifetimeManagement>();
            builder.Services.AddTransient<ITransientLifetimeManagement, TransientLifetimeManagement>();
            builder.Services.AddScoped<IDiTesterService, DiTesterService>();
        }

        private static IConfiguration GetConfiguration(IServiceProvider serviceProvider)
        {
            return GetConfiguration();
        }

        public static IConfiguration GetConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            try
            {
                var config = configBuilder.Build();
                if (IsDevelopment)
                {
                    // Use Cert Based Identity for app not deployed to Azure
                    configBuilder.AddAzureKeyVault(
                            $"https://{config["KeyVaultName"]}.vault.azure.net/",
                            config["AzureADApplicationId"],
                            config.GetClientCert());
                }
                else
                {
                    // Use Managed Identity when deployed to Azure
                    AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
                    KeyVaultClient keyVaultClient = new KeyVaultClient(
                        new KeyVaultClient.AuthenticationCallback(
                            azureServiceTokenProvider.KeyVaultTokenCallback));

                    configBuilder.AddAzureKeyVault(
                            $"https://{config["KeyVaultName"]}.vault.azure.net/",
                            keyVaultClient,
                            new DefaultKeyVaultSecretManager());
                }
            }
            catch
            {

            }

            return configBuilder.Build();
        }
    }

}
