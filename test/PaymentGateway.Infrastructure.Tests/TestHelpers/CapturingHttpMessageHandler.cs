namespace PaymentGateway.Infrastructure.Tests.TestHelpers;

public class CapturingHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    public HttpRequestMessage? CapturedRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        CapturedRequest = request;
        return Task.FromResult(response);
    }
}