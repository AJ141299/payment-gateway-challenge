using PaymentGateway.Core.Interfaces.Clients.Models;

namespace PaymentGateway.Core.Interfaces.Clients;

public interface IBankClient
{
    Task<BankPaymentResponse?> AuthorizePaymentAsync(BankPaymentRequest request, CancellationToken ct);
}