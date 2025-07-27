using BackgroundDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<SampleData>();
builder.Services.AddHostedService<BackgroundRefresh>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// An endpoint to retrieve the data from SampleData.
app.MapGet("/data", (SampleData data) => data.Data.Order());

app.Run();