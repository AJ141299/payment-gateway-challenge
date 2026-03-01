using System.Text.Json;

namespace PaymentGateway.Api.Clients;

public class BankClient(HttpClient client) : IBankClient
{
    public async Task<BankPaymentResponse?> MakePaymentAsync(BankPaymentRequest request, CancellationToken ct = default)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        var response = await client.PostAsJsonAsync("payments", request, options, ct);
        
        response.EnsureSuccessStatusCode();
        
        return response.Content.ReadFromJsonAsync<BankPaymentResponse>(cancellationToken: ct).Result;
    }
}

public interface IBankClient
{
    Task<BankPaymentResponse?> MakePaymentAsync(BankPaymentRequest request, CancellationToken ct);
}

public record BankPaymentRequest
{
    public required string CardNumber { get; init; }
    public required string ExpiryDate { get; init; }
    public required string Currency { get; init; }
    public required int Amount { get; init; }
    public required string Cvv { get; init; }
}

public class BankPaymentResponse
{
    public required string Authorized { get; init; }
    public required string AuthorizationCode { get; init; }
}