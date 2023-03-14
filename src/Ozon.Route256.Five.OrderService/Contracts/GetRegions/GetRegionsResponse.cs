namespace Ozon.Route256.Five.OrderService.Contracts.GetRegions;

/// <summary>
/// Получение списка регионов
/// </summary>
/// <param name="Regions">Регионы</param>
public record GetRegionsResponse(IReadOnlyList<string> Regions);