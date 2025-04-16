using Confluent.Kafka;
using System.Text.Json;
using UptimeMonitoringAPI.Models;

namespace UptimeMonitoringAPI.Services;

public static class KafkaProducer
{
    private static readonly IProducer<Null, string> _producer = new ProducerBuilder<Null, string>(
        new ProducerConfig 
        { 
            BootstrapServers = "kafka:9092",
            Acks = Acks.All
        }).Build();

    public static void Produce(string topic, MonitorRequest req)
    {
        var message = JsonSerializer.Serialize(req);
        try
        {
            Console.WriteLine($"Producing message to topic '{topic}': {message}");
            _producer.Produce(topic, new Message<Null, string> { Value = message }, (deliveryReport) =>
            {
                if (deliveryReport.Error.Code != ErrorCode.NoError)
                {
                    Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                }
                else
                {
                    Console.WriteLine($"Message delivered to {deliveryReport.TopicPartitionOffset}");
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while producing message: {ex.Message}");
            throw;
        }
    }
}