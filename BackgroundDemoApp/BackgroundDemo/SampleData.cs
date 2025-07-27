using System.Collections.Concurrent;

namespace BackgroundDemo;

public class SampleData
{
    // ConccurrentBag is used to allow multiple threads to add data concurrently.
    public ConcurrentBag<string> Data { get; } = new ConcurrentBag<string>();
}
