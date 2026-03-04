namespace PaymentGateway.Core.Interfaces.Clients.Models;

public class BankPaymentResponse
{
    public required bool Authorized { get; init; }
    public required string AuthorizationCode { get; init; }
}