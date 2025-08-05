# Background Jobs in ASP.NET Core - Using IHostedService
<img width="820" height="600" alt="image" src="https://github.com/user-attachments/assets/5484a21f-f52d-4bcd-945d-1766363a90ee" />

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
