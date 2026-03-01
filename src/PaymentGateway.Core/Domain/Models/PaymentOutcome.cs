using PaymentGateway.Core.Domain.Enums;

namespace PaymentGateway.Core.Domain.Models;

public class PaymentOutcome
{
    public required string Id { get; set; }
    public required PaymentStatus Status { get; set; }
    public required string CardNumberLastFour { get; set; }
    public required int ExpiryMonth { get; set; }
    public required int ExpiryYear { get; set; }
    public required string CurrencyIso3 { get; set; }
    public required ulong Amount { get; set; }
}