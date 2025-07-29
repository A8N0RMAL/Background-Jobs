using Quartz;
using QuartzDemo.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Quartz
builder.Services.AddQuartz(q =>
{
    // Use a simple GUID for the job key
    var jobKey = new JobKey("SampleJob");

    // Register our job
    q.AddJob<HelloWorldJob>(opts => opts.WithIdentity(jobKey));

    // Configure trigger (runs every 5 seconds)
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("SampleJob-Trigger")
        .WithSimpleSchedule(s => s
            .WithIntervalInSeconds(5)
            .RepeatForever()));
});

// Add Quartz hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();