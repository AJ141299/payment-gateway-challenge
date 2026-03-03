using System.Net;
using System.Text.Json;

namespace PaymentGateway.IntegrationTests.TestHelpers;

public class FakeBankHttpHandler : HttpMessageHandler
{
    private bool _authorized;
    private string _authCode = string.Empty;

    public void RespondWith(bool authorized, string authCode)
    {
        _authorized = authorized;
        _authCode = authCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var body = JsonSerializer.Serialize(new
        {
            authorized = _authorized,
            authorization_code = _authCode
        });

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
        });
    }
}
