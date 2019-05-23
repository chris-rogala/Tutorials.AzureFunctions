using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tutorials.AzureFunctions.DependencyInjection.Core;

namespace Tutorials.AzureFunctions.DependencyInjection.HttpTriggers
{
    public class Function1
    {
        private IDiTesterService _diTesterService;

        public Function1(IDiTesterService diTesterService,
            IScopedLifetimeManagement scopedLifetimeManagement)
        {
            _diTesterService = diTesterService ?? throw new ArgumentNullException(nameof(diTesterService));
            scopedLifetimeManagement.Add("Constructed-Function1");
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null ?
                (ActionResult)new OkObjectResult(_diTesterService.Run(name, req.Path))
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
