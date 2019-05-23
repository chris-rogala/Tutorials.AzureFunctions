using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Tutorials.AzureFunctions.KeyVaultEnabledConfig.Functions
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            var configuration = GetConfiguration();
            return name != null
                ? (ActionResult)new OkObjectResult(new { key = name, value = configuration[name] })
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        #region Get certificate
        private const string _clientThumbprint = "clientThumbprint";
        public static X509Certificate2 GetClientCert(this IConfiguration configuration)
        {
            using (X509Store str = new X509Store())
            {
                var thumbprint = configuration[_clientThumbprint]?.ToUpper()?.Trim();
                str.Open(OpenFlags.ReadOnly);
                X509Certificate2 result = str.Certificates
                    .OfType<X509Certificate2>()
                    .FirstOrDefault(x => x.Thumbprint.ToUpper().Trim() == thumbprint);
                str.Close();
                return result;
            }
        }
        #endregion Get certificate

        #region Instantiate IConfiguration
        private const string _keyVaultName = "KeyVaultName";
        private const string _azureADApplicationId = "AzureADApplicationId";

        public static IConfiguration GetConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
               .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
               .AddEnvironmentVariables();

            var config = configBuilder.Build();
            configBuilder.AddAzureKeyVault(
                        $"https://{config[_keyVaultName]}.vault.azure.net/",
                        config[_azureADApplicationId],
                        config.GetClientCert());

            return configBuilder.Build();
        }
        #endregion Instantiate IConfiguration
    }
}
