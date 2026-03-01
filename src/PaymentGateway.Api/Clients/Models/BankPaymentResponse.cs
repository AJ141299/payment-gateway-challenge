namespace PaymentGateway.Api.Clients.Models;

public class BankPaymentResponse
{
    public required string Authorized { get; init; }
    public required string AuthorizationCode { get; init; }
}