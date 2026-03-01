using System.Net.Http.Json;
using System.Text.Json;
using PaymentGateway.Core.Interfaces.Clients;
using PaymentGateway.Core.Interfaces.Clients.Models;

namespace PaymentGateway.Infrastructure.Clients;

public class BankClient(HttpClient client) : IBankClient
{
    public async Task<BankPaymentResponse?> AuthorizePaymentAsync(BankPaymentRequest request, CancellationToken ct = default)
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

            var response = await client.PostAsJsonAsync("payments", request, options, ct);

            response.EnsureSuccessStatusCode();

            return response.Content.ReadFromJsonAsync<BankPaymentResponse>(cancellationToken: ct).Result;
        }
        catch (Exception e)
        {
            // TODO: we should log the exception
            return null;
        }
    }
}