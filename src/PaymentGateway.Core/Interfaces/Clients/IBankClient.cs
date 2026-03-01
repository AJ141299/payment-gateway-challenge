using PaymentGateway.Core.Interfaces.Clients.Models;

namespace PaymentGateway.Core.Interfaces.Clients;

public interface IBankClient
{
    Task<BankPaymentResponse?> MakePaymentAsync(BankPaymentRequest request, CancellationToken ct);
}