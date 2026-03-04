namespace PaymentGateway.Infrastructure.Tests.TestHelpers;

public class MockHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        => Task.FromResult(response);
}