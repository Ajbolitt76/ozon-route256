using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

public class ConsumerSettings
{
    public string? Topic { get; set; }
    
    public bool Enabled { get; set; } = true;

    public bool AutoCommit => !PerformSynchronousCommit;

    public bool PerformSynchronousCommit { get; set; } = true;

    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Latest;
}