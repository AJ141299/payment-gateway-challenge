namespace PaymentGateway.Core.Domain.Models;

public class PaymentDetails
{
    public required string CardNumber { get; set; }
    public required int ExpiryMonth { get; set; }
    public required int ExpiryYear { get; set; }
    public required string Currency { get; set; }
    public required ulong Amount { get; set; }
    public required string Cvv { get; init; }
}