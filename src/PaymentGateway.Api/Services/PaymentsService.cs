using PaymentGateway.Api.Clients;

namespace PaymentGateway.Api.Services;

public class PaymentsService(IBankClient bankClient) : IPaymentsService
{
    public async Task MakePaymentAsync()
    {
        
    }
}

public interface IPaymentsService
{
}