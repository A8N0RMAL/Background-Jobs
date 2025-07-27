
namespace BackgroundDemo;

// IHostedService is used to run background tasks in ASP.NET Core applications.
public class BackgroundRefresh : IHostedService, IDisposable
{
    private Timer? _timer;
    private readonly SampleData _data;
    public BackgroundRefresh(SampleData data)
    {
        _data = data;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Initialize the timer to call the AddToCache method every 1 second.
        _timer = new Timer(AddToCache, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        return Task.CompletedTask;
    }

    private void AddToCache(object? state)
    {
        // This method is called by the timer every second.
        // It adds a new entry to the SampleData cache.
        _data.Data.Add("Added new data to cache at: " + DateTime.Now.ToLongTimeString());
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop the timer when the application is shutting down.
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
    public void Dispose()
    {
        // Dispose of the timer when the service is disposed.
        _timer?.Dispose();
        _timer = null;
    }
}
