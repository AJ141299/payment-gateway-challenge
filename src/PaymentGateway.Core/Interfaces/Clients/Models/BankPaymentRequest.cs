using PaymentGateway.Core.Domain.Models;

namespace PaymentGateway.Core.Interfaces.Clients.Models;

public class BankPaymentRequest
{
    public required string CardNumber { get; init; }
    public required string ExpiryDate { get; init; }
    public required string Currency { get; init; }
    public required ulong Amount { get; init; }
    public required string Cvv { get; init; }

    public static BankPaymentRequest FromDomain(PaymentDetails source) => new()
    {
        CardNumber = source.CardNumber,
        ExpiryDate = $"{source.ExpiryMonth:D2}/{source.ExpiryYear}",
        Currency = source.CurrencyIso3,
        Amount = source.Amount,
        Cvv = source.Cvv
    };
}