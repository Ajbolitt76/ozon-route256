using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Cqrs;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Features.CancelOrder;

public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand>
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;

    public CancelOrderCommandHandler(
        LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
    {
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
    }
    
    public Task<HandlerResult> Handle(CancelOrderCommand request, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}