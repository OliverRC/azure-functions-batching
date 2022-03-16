using System.Collections.Generic;

namespace Nether.Function.Batching;

public class Job
{
    public IEnumerable<Car> Cars { get; set; }
}