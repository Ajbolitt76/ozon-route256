using Grpc.Core;
using Ozon.Route256.Five.OrderService.Mappings;

namespace Ozon.Route256.Five.OrderService.DbClientBalancer;

public class SdConsumerHostedService : BackgroundService
{
    private readonly IDbStore _dbStore;
    private readonly SdService.SdServiceClient _client;
    private readonly ILogger<SdConsumerHostedService> _logger;
    private readonly TimeSpan _retryDelayMs = TimeSpan.FromMilliseconds(1000);

    public SdConsumerHostedService(
        IDbStore dbStore,
        SdService.SdServiceClient client,
        ILogger<SdConsumerHostedService> logger)
    {
        _dbStore = dbStore;
        _client = client;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var stream = _client.DbResources(
                    new DbResourcesRequest
                    {
                        ClusterName = "cluster"
                    },
                    cancellationToken: stoppingToken);
                
                while (await stream.ResponseStream.MoveNext(stoppingToken))
                {
                    _logger.LogDebug(
                        "Получены новые даныне из SD. Timestamp {Timestamp}",
                        stream.ResponseStream.Current.LastUpdated.ToDateTime());
                    var response = stream.ResponseStream.Current;

                    var endpoints = new List<DbEndpoint>(response.Replicas.Capacity);

                    foreach (var replica in response.Replicas)
                    {
                        var endpoint = new DbEndpoint($"{replica.Host}:{replica.Port}", replica.Type.ToModel());
                        endpoints.Add(endpoint);
                    }

                    await _dbStore.SetEndpointList(endpoints);
                }
            }
            catch (RpcException exc)
            {
                _logger.LogError(
                    exc,
                    "Не удалось связаться с SD. Повторная попытка переподключения через {RetryTime} мс",
                    _retryDelayMs.TotalMilliseconds);
            }

            await Task.Delay(_retryDelayMs);
        }
    }
}