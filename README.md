# Background Jobs in ASP.NET Core
# 1. Background Jobs in ASP.NET Core - Using IHostedService
This project shows how to create background tasks in ASP.NET Core web applications using the built-in `IHostedService`.

## Why This Matters

Background jobs are used for:
- Sending emails without slowing down your app
- Processing files in the background
- Regular cleanup tasks
- Updating cached data

## What This Does

1. **Runs a background job** that adds data every second
2. **Stores the data safely** where multiple threads can access it
3. **Shows the results** through a web API endpoint

## Main Parts

### 1. Background Service (`BackgroundRefresh.cs`)
```csharp
// 1. Service starts
public Task StartAsync(CancellationToken cancellationToken)
{
    _timer = new Timer(AddToCache, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    return Task.CompletedTask;
}

// 2. The actual work
private void AddToCache(object? state)
{
    _data.Data.Add($"Added at: {DateTime.Now.ToLongTimeString()}");
}

// 3. Service stops
public Task StopAsync(CancellationToken cancellationToken)
{
    _timer?.Change(Timeout.Infinite, 0);
    return Task.CompletedTask;
}
```
1. **Start**: Creates a timer that runs every second
2. **Execute**: Adds timestamped message to storage
3. **Stop**: Cleans up when app shuts down
---

### 2. Thread-Safe Data Storage (SampleData.cs)
```csharp
public class SampleData
{
    public ConcurrentBag<string> Data { get; } = new();
}
```
- `ConcurrentBag`: Special list that's safe for:
  - Multiple threads adding data at once
  - No crashes from simultaneous access
  - Built-in .NET solution for thread safety

---
### 3. Web API (`Program.cs`)
```csharp
// Register services
builder.Services.AddSingleton<SampleData>(); // Shared data store
builder.Services.AddHostedService<BackgroundRefresh>(); // Background worker

// API endpoint to view results
app.MapGet("/data", (SampleData data) => data.Data.Order());
```
- `AddSingleton`: Creates one shared data store for the whole app
- `AddHostedService`: Registers our background worker
- `MapGet`: Creates an endpoint to check the stored data
---

## Output

After running the application, the background service automatically adds timestamped entries every second. You can view these entries by accessing the `/data` endpoint:

<img width="1602" height="425" alt="Screenshot 2025-07-28 011049" src="https://github.com/user-attachments/assets/0fafe77d-6299-4d27-81ae-c55f9f471201" />

**Key observations from the output:**
- The service successfully adds new entries every second (visible by the timestamps)
- All entries are stored in thread-safe concurrent storage
- The API endpoint returns all collected data in order
- The timestamps show continuous operation (01:09:48 through 01:10:00)

---

# 2. Background Jobs in ASP.NET Core - Using Hangfire
## Solution Architecture
This solution demonstrates an implementation of background job processing using Hangfire in an ASP.NET Core Web API application.

### 1. Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=HangfireDB;..."
  }
}
```
- **Database Configuration**: Configures SQL Server LocalDB as the persistent storage for Hangfire jobs
- **Persistence**: Ensures jobs survive application restarts and server crashes
- **Isolation**: Uses a dedicated database (HangfireDB) for job storage
---

### 2. Service Registration (Program.cs)
```csharp
// Hangfire Service Configuration
builder.Services.AddHangfire(x => 
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

// Dashboard Configuration
app.UseHangfireDashboard("/hangfiredashboard");
```
- **Job Storage**: Configures SQL Server as the backing store for jobs
- **Processing Server**: Initializes the Hangfire server component
- **Monitoring**: Enables the Hangfire dashboard at `/hangfiredashboard`
- **Scalability**: Ready for horizontal scaling with multiple servers
---

# Hangfire Background Jobs Controller Overview
This controller demonstrates three fundamental types of background job processing using Hangfire in an ASP.NET Core Web API application.

## Job Method Implementation

```csharp
[ApiExplorerSettings(IgnoreApi = true)]
public void SendEmail(string email)
{
    Console.WriteLine($"Email sent to {email} at {DateTime.Now}");
}
```

---
## 1. Fire-and-Forget Job (Enqueue)

```csharp
[HttpPost("enqueue")]
public IActionResult EnqueueJob()
{
    BackgroundJob.Enqueue(() => SendEmail("LuciferXD@outlook.com"));
    return Ok("Job enqueued!");
}
```
### Output
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/f7a7218a-f7fb-42c6-8cdf-99ef07db43cc" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/d697f382-19d0-4988-b9f3-7547c2975d22" />

### Characteristics:
- **Immediate Execution**: Runs as soon as possible
- **Single Execution**: Processes exactly once
- **Queue-Based**: Added to Hangfire's processing queue

### Use Cases:
- Email notifications
- Instant data processing
- Non-critical background tasks
---

## 2. Scheduled Job (Delayed Execution)

```csharp
[HttpPost("schedule")]
public IActionResult ScheduleJob()
{
    Console.WriteLine(DateTime.Now);
    BackgroundJob.Schedule(() => SendEmail("LuciferXD@outlook.com"), TimeSpan.FromMinutes(3));
    return Ok("Job scheduled!");
}
```
### Output
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/145da221-3702-4dbd-9b17-102135d29cf8" />
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/98ea297e-0bcb-4b2a-958e-ba030f4fb487" />


### Characteristics:
- **Delayed Execution**: Runs after specified time (3 minutes)
- **Single Execution**: Processes exactly once at scheduled time
- **Persistent**: Survives application restarts

### Use Cases:
- Reminder systems
- Delayed notifications
- Scheduled maintenance tasks
---

## 3. Recurring Job (Periodic Execution)

```csharp
[HttpPost("start-recurring")]
public IActionResult StartRecurringUpdate()
{
    RecurringJob.AddOrUpdate(
        "send-email-minutely",
        () => SendEmail("LuciferXD@outlook.com"),
        Cron.Minutely);
    return Ok("Recurring job started!");
}
```
### Output
<img width="1920" height="1080" alt="image" src="https://github.com/user-attachments/assets/71577c70-bcb0-487f-ba17-45ba20f32837" />
<img width="1919" height="884" alt="image" src="https://github.com/user-attachments/assets/03018e93-0061-45c4-abed-e266f3ae1f15" />


### Characteristics:
- **Periodic Execution**: Runs every minute (configurable via cron)
- **Persistent Schedule**: Maintains schedule across restarts
- **Unique Identifier**: "send-email-minutely" prevents duplicates
- **Self-Healing**: Automatically recovers from failures

### Use Cases:
- Regular reports
- Data synchronization
- System monitoring

---
## Key Differences

| Feature          | Enqueue | Scheduled | Recurring |
|------------------|---------|-----------|-----------|
| Execution Time   | Now     | Future    | Repeated  |
| Runs             | Once    | Once      | Many      |
| Persistence      | Yes     | Yes       | Yes       |
| Unique ID        | No      | No        | Required  |

---

## Testing the Endpoints

1. **Enqueue Job**: POST to `/WeatherForecast/enqueue`
2. **Schedule Job**: POST to `/WeatherForecast/schedule`
3. **Recurring Job**: POST to `/WeatherForecast/start-recurring`

All jobs can be monitored via the Hangfire dashboard at `/hangfiredashboard`

---

# 3. Background Jobs in ASP.NET Core - Using Quartz.NET
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
