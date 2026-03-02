using PaymentGateway.Core.Domain.Models;

namespace PaymentGateway.Api.Models.Requests;

public class ProcessPaymentRequest
{
    public required string CardNumber { get; set; }
    public required int ExpiryMonth { get; set; }
    public required int ExpiryYear { get; set; }
    public required string Currency { get; set; }
    public required ulong Amount { get; set; }
    public required string Cvv { get; set; }

    public PaymentDetails ToPaymentDetails() => new()
    {
        CardNumber = CardNumber,
        ExpiryMonth = ExpiryMonth,
        ExpiryYear = ExpiryYear,
        CurrencyIso3 = Currency,
        Amount = Amount,
        Cvv = Cvv
    };
}