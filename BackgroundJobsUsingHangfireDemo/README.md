# Hangfire Background Jobs Implementation in ASP.NET Core
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

## Testing the Endpoints

1. **Enqueue Job**: POST to `/WeatherForecast/enqueue`
2. **Schedule Job**: POST to `/WeatherForecast/schedule`
3. **Recurring Job**: POST to `/WeatherForecast/start-recurring`

All jobs can be monitored via the Hangfire dashboard at `/hangfiredashboard`

---
