using System.Text.Json;

using PaymentGateway.Api.Clients.Models;

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