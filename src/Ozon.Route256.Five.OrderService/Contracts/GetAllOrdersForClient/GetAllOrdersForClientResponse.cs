using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.GetAllOrdersForClient;

/// <summary>
/// Результат запроса на список заказов для клиента <see cref="GetAllOrdersForClientRequest"/>
/// </summary>
public record GetAllOrdersForClientResponse(IReadOnlyCollection<GetAllOrdersForClientResponseItem> Orders);