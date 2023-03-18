using Ozon.Route256.Five.OrderService.Model;

namespace Ozon.Route256.Five.OrderService.Contracts.KafkaMessages.OrderEvents;

// TODO: Данные возвращаемые сервисом не соответствуют, описанию дз
public record OrderEventMessage(long OrderId, OrderState NewState, DateTime ChangedAt);