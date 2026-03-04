using PaymentGateway.Core.Domain.Models;

namespace PaymentGateway.Core.Services;

public interface IPaymentsService
{
    Task<Payment> ProcessPaymentAsync(PaymentDetails paymentDetails, CancellationToken ct = default);
    Payment? GetPayment(string id);
}