namespace Ozon.Route256.Five.OrderService.Services.Database;

public class ShardingOptions
{
    public const string ConnectionTemplateHostPattern = "{serverHost}";
    
    public string ConnectionTemplate { get; set; }

    public string GetConnectionString(string hostPort)
        => ConnectionTemplate.Replace(ConnectionTemplateHostPattern, hostPort);
}