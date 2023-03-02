namespace Ozon.Route256.Five.OrderService.Contracts.GetForRegions;

public record GetForRegionsRequest(DateTime? StartDate, List<string>? Regions);