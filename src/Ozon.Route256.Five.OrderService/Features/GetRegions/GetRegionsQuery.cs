using Ozon.Route256.Five.OrderService.Contracts.GetRegions;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetRegions;

public record GetRegionsQuery() : IQuery<GetRegionsResponse>;