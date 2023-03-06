namespace Ozon.Route256.Five.OrderService.Contracts;

/// <summary>
/// Описание ошибки
/// </summary>
/// <param name="Code">Код</param>
/// <param name="Description">Описание</param>
public record ErrorDescription(string Code, string Description);