using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Nether.Function.Batching;

public static class WorkerJobFunction
{
    [FunctionName("WorkerJobFunction")]
    public static async Task RunAsync([QueueTrigger("batch-jobs", Connection = "")] string jobItem, ILogger log)
    {
        log.LogInformation($"C# Queue trigger function processed");

        log.LogInformation($"JobItem: {jobItem}");
        
        var jobs = JsonConvert.DeserializeObject<IEnumerable<Job>>(jobItem);
        var output = string.Join(Environment.NewLine, jobs.Select(x => $"{x.From} - {x.To}"));
        log.LogInformation(output);
    }
}