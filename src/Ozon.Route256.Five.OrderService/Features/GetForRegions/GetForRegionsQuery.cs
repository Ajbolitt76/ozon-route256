using Ozon.Route256.Five.OrderService.Contracts.GetForRegions;
using Ozon.Route256.Five.OrderService.Cqrs;

namespace Ozon.Route256.Five.OrderService.Features.GetForRegions; 

public record GetForRegionsQuery(
    DateTime StartFrom,
    IReadOnlyList<string> Regions) : GetForRegionsRequest(StartFrom, Regions), IQuery<GetForRegionsResponse>;