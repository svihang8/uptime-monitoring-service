using Confluent.Kafka;
using System.Text.Json;
using UptimeMonitoringAPI.Models;

namespace UptimeMonitoringAPI.Services;

public class KafkaMonitorConsumer : BackgroundService
{
    private readonly IRedisService _redis;
    private readonly ILogger<KafkaMonitorConsumer> _logger;

    private readonly IConfiguration _configuration;

    public KafkaMonitorConsumer(IRedisService redis, ILogger<KafkaMonitorConsumer> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _redis = redis;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting Kafka consumer...");
        var config = new ConsumerConfig
        {
            BootstrapServers = $"{_configuration.GetValue<string>("Host:Kafka")}",
            GroupId = "monitor-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            EnableAutoOffsetStore = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        
        try
        {
            _logger.LogInformation("Subscribing to topic check-requests...");
            consumer.Subscribe("check-requests");
            _logger.LogInformation("Successfully subscribed to topic");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (result == null) {
                        _logger.LogDebug("No Kafka message received.");
                        await Task.Delay(1000, stoppingToken); // Add a real delay
                        continue;
                    }
                    ;

                    _logger.LogInformation($"Received message: {result.Message.Value}");
                    var request = JsonSerializer.Deserialize<MonitorRequest>(result.Message.Value);
                    
                    if (request != null && !string.IsNullOrWhiteSpace(request.Url))
                    {
                        _logger.LogInformation($"Processing URL: {request.Url}");
                        await ProcessUrlAsync(request.Url);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError($"Error consuming message: {ex.Error.Reason}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing message: {ex.Message}");
                }
            }
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Consumer closed");
        }
    }

    private async Task ProcessUrlAsync(string url)
    {
        var client = new HttpClient();
        var start = DateTime.UtcNow;
        try
        {
            var res = await client.GetAsync(url);
            var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
            _logger.LogInformation($"URL {url} is up. Response time: {elapsed}ms");
            await _redis.SaveStatus(url, true, elapsed);
        }
        catch (Exception ex)
        {
            _logger.LogError($"URL {url} is down. Error: {ex.Message}");
            await _redis.SaveStatus(url, false, 0);
        }
    }
}