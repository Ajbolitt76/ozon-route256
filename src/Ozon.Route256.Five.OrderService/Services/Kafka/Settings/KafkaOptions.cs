using System.ComponentModel.DataAnnotations;
using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Services.Kafka.Settings;

public class KafkaOptions
{
    [Required]
    public string? BootstrapServers { get; set; }
    
    [Required]
    public string? GroupId { get; set; }
    
    public int TimeoutForRetryInSecond { get; set; } = 2;
    
    public Acks Acks { get; set; } = Acks.Leader;
    
    public bool EnableIdempotence { get; set; } = false;

    [Required]
    public Dictionary<string, ConsumerSettings> ConsumerSettings { get; set; } = new();
    
    [Required]
    public Dictionary<string, ProducerSettings> ProducerSettings { get; set; } = new();
}