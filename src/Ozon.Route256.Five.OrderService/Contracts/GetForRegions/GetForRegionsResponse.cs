namespace Ozon.Route256.Five.OrderService.Contracts.GetForRegions;

/// <summary>
/// Результат запроса <see cref="GetForRegionsRequest"/>
/// </summary>
/// <param name="RegionStats">Результаты по регионам</param>
public record GetForRegionsResponse(IReadOnlyList<GetForRegionsResponseItem> RegionStats);
