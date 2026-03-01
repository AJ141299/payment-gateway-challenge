using PaymentGateway.Core.Interfaces;

namespace PaymentGateway.Core.Services;

public class PaymentsService(IBankClient bankClient) : IPaymentsService
{
    public async Task MakePaymentAsync()
    {
        
    }
}