using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Newtonsoft.Json;

namespace Nether.Function.Batching;

public static class ManualJobFunction
{
    [FunctionName("ManualJobFunction")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, 
        [Queue("batch-jobs")]IAsyncCollector<string> batchJobsQueue, 
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");
        
        var batchSize = Int32.Parse(req.Query["batchSize"]);
        
        var cars = new List<Car>
        {
            new Car{ Id = 1, Make = "Dodge", Model = "Caliber" },
            new Car{ Id = 2, Make = "GMC", Model = "Rally Wagon 1500" },
            new Car{ Id = 3, Make = "Kia", Model = "Optima" },
            new Car{ Id = 4, Make = "Porsche", Model = "911" },
            new Car{ Id = 5, Make = "Mercedes", Model = "Benz,Sprinter 3500" },
            new Car{ Id = 6, Make = "Volvo", Model = "V40" },
            new Car{ Id = 7, Make = "Mercedes", Model = "Benz,GL-Class" },
            new Car{ Id = 8, Make = "GMC", Model = "Sierra 3500" },
            new Car{ Id = 9, Make = "Chevrolet", Model = "Corvette" },
            new Car{ Id = 10, Make = "Chevrolet", Model = "Cobalt" },
            new Car{ Id = 11, Make = "Buick", Model = "Skylark" },
            new Car{ Id = 12, Make = "Pontiac", Model = "Safari" },
            new Car{ Id = 13, Make = "Volvo", Model = "C30" },
            new Car{ Id = 14, Make = "Kia", Model = "Optima" },
            new Car{ Id = 15, Make = "Acura", Model = "TL" },
            new Car{ Id = 16, Make = "Porsche", Model = "Cayenne" },
            new Car{ Id = 17, Make = "Dodge", Model = "Caravan" },
            new Car{ Id = 18, Make = "Hyundai", Model = "Sonata" },
            new Car{ Id = 19, Make = "Ford", Model = "Mustang" },
            new Car{ Id = 20, Make = "Subaru", Model = "Impreza" },
        };

        var batches = cars.Batch(batchSize).ToList();

        foreach (var batch in batches)
        {
            var jsonString = JsonConvert.SerializeObject(batch);
            await batchJobsQueue.AddAsync(jsonString);
        }

        return new OkObjectResult($"{batches.Count} batches queued up.");
    }
}