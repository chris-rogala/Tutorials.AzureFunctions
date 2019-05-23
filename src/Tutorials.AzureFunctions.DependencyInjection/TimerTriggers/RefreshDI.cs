using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Tutorials.AzureFunctions.DependencyInjection.TimerTriggers
{
    public class RefreshDI
    {
        private IConfiguration _configuration;

        public RefreshDI(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("RefreshDI")]
        public void Run([TimerTrigger("*/15 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            (_configuration as IConfigurationRoot).Reload();
        }
    }
}
