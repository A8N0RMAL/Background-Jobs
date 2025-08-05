# **Quartz.NET in ASP.NET Core**
<img width="1261" height="796" alt="QuartzNet Concept Map (1)" src="https://github.com/user-attachments/assets/f6fc7391-15ba-4b7b-86ca-3cc8d6eca4ab" />

## **What Quartz.NET Does in ASP.NET Core**

Quartz.NET is a powerful, enterprise-grade job scheduling system that enables you to:
- **Schedule background jobs** to run at precise times or intervals
- **Execute recurring tasks** using cron expressions or simple intervals
- **Manage long-running processes** independently of HTTP requests
- **Persist job state** across application restarts
- **Cluster jobs** across multiple application instances
- **Handle job failures** with retry policies

In ASP.NET Core, Quartz integrates seamlessly with:
- Dependency Injection system
- Configuration system
- Logging infrastructure
- Application lifecycle management

## **Getting Started**

1. Install packages:
   ```powershell
   Install-Package Quartz
   Install-Package Quartz.Extensions.Hosting
   ```

2. Implement your job class (like `HelloWorldJob`)

3. Configure the scheduler in `Program.cs`

4. Add any needed persistence or monitoring
---

## **1. Job Implementation (`HelloWorldJob.cs`)**

```csharp
using Quartz;
using Microsoft.Extensions.Logging;

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
```

### **Key Components**

| Component | Purpose | Quartz Integration |
|-----------|---------|--------------------|
| `IJob` | Defines executable work unit | Core interface |
| `JobExecutionContext` | Provides execution context | Contains job data, trigger info |
| `ILogger` | Structured logging | ASP.NET Core DI |

### **Best Practices**
- Keep jobs focused on single responsibility
- Make jobs stateless for reliability
- Use constructor injection for dependencies
- Implement proper error handling

## **2. Service Configuration (`Program.cs`)**

```csharp
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("SampleJob");
    
    q.AddJob<HelloWorldJob>(opts => opts.WithIdentity(jobKey));
    
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SampleJob-Trigger")
        .WithSimpleSchedule(s => s
            .WithIntervalInSeconds(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
```

### **Configuration Breakdown**

| Method | Purpose | Production Considerations |
|--------|---------|---------------------------|
| `AddJob` | Registers job type | Use meaningful job keys |
| `AddTrigger` | Defines execution schedule | Consider misfire policies |
| `AddQuartzHostedService` | Runs scheduler in background | Configure shutdown behavior |

---

## **Job Execution Output**

<img width="1919" height="1025" alt="Screenshot 2025-07-29 032801" src="https://github.com/user-attachments/assets/27692f7e-3082-495d-b17b-8d7b60ff8681" />
This output demonstrates Quartz.NET successfully executing the `HelloWorldJob` every 5 seconds.

---
