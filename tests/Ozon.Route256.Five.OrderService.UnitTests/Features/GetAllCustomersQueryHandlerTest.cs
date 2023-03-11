using FluentAssertions;
using Ozon.Route256.Five.OrderService.Contracts.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Cqrs.ResultTypes;
using Ozon.Route256.Five.OrderService.Features.GetAllCustomers;
using Ozon.Route256.Five.OrderService.Mappings;
using Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;
using Ozon.Route256.Five.OrderService.UnitTests.Extensions;

namespace Ozon.Route256.Five.OrderService.UnitTests.Features;

public class GetAllCustomersQueryHandlerTest
{
    /// <summary>
    /// Получение покупателей, проксирует на сервис покупателей
    /// </summary>
    [Fact]
    public async Task Handle_CancelOrder_ShouldProxyToCustomerService()
    {
        var customers = FakeDataGenerators.CustomerServiceCustomerDtos.Take(10).ToList();

        var customersMock = CustomerServiceMock.WithGetCustomersData(customers);

        var handler = new GetAllCustomerQueryHandler(customersMock.Object);
        var result = await handler.Handle(new GetAllCustomerQuery(), default);

        result
            .Should()
            .BeSuccessful()
            .WhichRequiredResult()
            .Customers
            .Should().Equal(customers.Select(
                x => new GetAllCustomersResponseItem(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.MobileNumber,
                    x.Email,
                    x.DefaultAddress.ToModel())).ToList());
    }
}