using PaymentGateway.Core.Interfaces.Models;

namespace PaymentGateway.Core.Interfaces;

public interface IBankClient
{
    Task<BankPaymentResponse?> MakePaymentAsync(BankPaymentRequest request, CancellationToken ct);
}