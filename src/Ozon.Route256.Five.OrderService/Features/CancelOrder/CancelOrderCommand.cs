using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.CancelOrder;

public record CancelOrderCommand(long Id) : ICommand;
