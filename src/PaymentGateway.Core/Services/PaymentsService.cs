using PaymentGateway.Core.Domain.Models;
using PaymentGateway.Core.Interfaces.Clients;
using PaymentGateway.Core.Interfaces.Repositories;

namespace PaymentGateway.Core.Services;

public class PaymentsService(IBankClient bankClient, IPaymentsRepository paymentsRepository) : IPaymentsService
{
    public async Task MakePaymentAsync(PaymentRequest paymentRequest, CancellationToken ct = default)
    {
    }
}