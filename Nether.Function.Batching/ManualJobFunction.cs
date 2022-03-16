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
        var numberOfMonths = 38;
        var batchSizeInMonths = 3;
        
        log.LogInformation("C# HTTP trigger function processed a request.");

        var today = DateTime.Now;
        var months = new List<DateTime>(); 
        for (int i = 0; i < numberOfMonths; i++)
        {
            months.Add(today.AddMonths(-1 * i));
        }

        var jobs = new List<Job>();
        foreach (var month in months)
        {
            var from = new DateTime(month.Year, month.Month, 1).ToString("yyyy/MM/dd");
            var to = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month)).ToString("yyyy/MM/dd");
            
            jobs.Add(new Job
            {
                From = from, To = to
            });
        }

        var batches = jobs.Batch(batchSizeInMonths).ToList();

        foreach (var batch in batches)
        {
            var jsonString = JsonConvert.SerializeObject(batch);
            await batchJobsQueue.AddAsync(jsonString);
        }

        return new OkObjectResult($"{batches.Count} batches queued up.");
    }
}