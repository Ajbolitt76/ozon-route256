namespace Ozon.Route256.Five.OrderService.Exceptions;

public class NotReadyException : Exception
{
    public NotReadyException() : base("Данная часть приложения не готова к работе")
    {

    }
}