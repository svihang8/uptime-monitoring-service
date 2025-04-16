using UptimeMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
Console.WriteLine("Registering services...");
builder.Services.AddSingleton<IRedisService, RedisService>();
Console.WriteLine("Registering Kafka background service...");
builder.Services.AddHostedService<KafkaMonitorConsumer>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();

builder.Services.AddOpenApi();

Console.WriteLine("Building app...");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options => {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

Console.WriteLine("Mapping controllers...");
app.MapControllers();

// Add detailed logging for Kestrel

Console.WriteLine("Running app...");
app.Run();
