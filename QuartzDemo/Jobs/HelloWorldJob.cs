using Quartz;

namespace QuartzDemo.Jobs;

public class HelloWorldJob : IJob
{
    private readonly ILogger<HelloWorldJob> _logger;
    public HelloWorldJob(ILogger<HelloWorldJob> logger)
    {
        _logger = logger;
    }
    public Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Hello from Quartz.NET! Time: {0}", DateTime.Now);
        return Task.CompletedTask;
    }
}
