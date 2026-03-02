
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Core.Domain.Models;

namespace PaymentGateway.Api.Models.Responses;

public class ProcessPaymentResponse
{
    public required string Id { get; set; }
    public required PaymentStatus Status { get; set; }
    public required string CardNumberLastFour { get; set; }
    public required int ExpiryMonth { get; set; }
    public required int ExpiryYear { get; set; }
    public required string Currency { get; set; }
    public required ulong Amount { get; set; }

    public static ProcessPaymentResponse FromPayment(Payment payment) => new()
    {
        Id = payment.Id,
        Status = payment.Status,
        CardNumberLastFour = payment.CardNumberLastFour,
        ExpiryMonth = payment.ExpiryMonth,
        ExpiryYear = payment.ExpiryYear,
        Currency = payment.Currency,
        Amount = payment.Amount
    };
}
