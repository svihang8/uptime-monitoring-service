using UptimeMonitoringAPI.Services;

var builder = WebApplication.CreateBuilder(args);


Console.WriteLine("Registering services...");

Console.WriteLine("Registering Redis service...");
builder.Services.AddSingleton<IRedisService, RedisService>();
Console.WriteLine("Registering Kafka background service...");
builder.Services.AddHostedService<KafkaMonitorConsumer>();

// OpenAPI Configuration
builder.Services.AddOpenApi();
builder.Services.AddControllers();

Console.WriteLine("Building app...");
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}

app.UseAuthorization();

Console.WriteLine("Mapping controllers...");
app.MapControllers();

Console.WriteLine("Running app...");
app.Run();
