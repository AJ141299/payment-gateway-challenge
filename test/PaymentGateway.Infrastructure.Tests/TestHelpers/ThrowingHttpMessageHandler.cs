namespace PaymentGateway.Infrastructure.Tests.TestHelpers;

public class ThrowingHttpMessageHandler(Exception exception) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        => Task.FromException<HttpResponseMessage>(exception);
}