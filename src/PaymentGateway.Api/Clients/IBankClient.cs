using PaymentGateway.Api.Clients.Models;

namespace PaymentGateway.Api.Clients;

public interface IBankClient
{
    Task<BankPaymentResponse?> MakePaymentAsync(BankPaymentRequest request, CancellationToken ct);
}