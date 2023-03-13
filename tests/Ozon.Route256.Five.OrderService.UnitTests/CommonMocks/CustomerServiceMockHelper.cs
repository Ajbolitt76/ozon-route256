using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Moq;
using Ozon.Route256.Five.CustomersService.Grpc;
using Ozon.Route256.Five.OrderService.UnitTests.Grpc;

namespace Ozon.Route256.Five.OrderService.UnitTests.CommonMocks;

public static class CustomerServiceMockHelper
{
    public static Mock<Customers.CustomersClient> WithGetCustomersData(IEnumerable<Customer> customers)
    {
        var customersMock = new Mock<Customers.CustomersClient>();
        customersMock.Setup(
                x => x.GetCustomersAsync(
                    It.IsAny<Empty>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(
                GrpcUtils.CreateAsyncUnaryCall(
                    new GetCustomersResponse()
                    {
                        Customers = { customers }
                    }));
        return customersMock;
    }
    
    public static Mock<Customers.CustomersClient> WithGetCustomerData(Customer customer)
    {
        var customersMock = new Mock<Customers.CustomersClient>();
        customersMock.Setup(
                x => x.GetCustomerAsync(
                    It.IsAny<GetCustomerByIdRequest>(),
                    It.IsAny<Metadata?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()))
            .Returns(
                GrpcUtils.CreateAsyncUnaryCall(customer));
        return customersMock;
    }
}