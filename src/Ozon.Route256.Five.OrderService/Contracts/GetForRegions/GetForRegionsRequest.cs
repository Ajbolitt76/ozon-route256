namespace Ozon.Route256.Five.OrderService.Contracts.GetForRegions;

/// <summary>
/// Запрос на аггрегацию заказов по регионам
/// </summary>
/// <param name="StartDate">Дата с которой начинать аггрегировать</param>
/// <param name="Regions">Список регионов</param>
public record GetForRegionsRequest(DateTime StartFrom, IReadOnlyList<string> Regions);