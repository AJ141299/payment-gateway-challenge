using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PaymentGateway.Core.Interfaces.Clients.Models;
using PaymentGateway.Infrastructure.Clients;
using PaymentGateway.Infrastructure.Tests.TestHelpers;

using Shouldly;

namespace PaymentGateway.Infrastructure.Tests.Clients;

public class BankClientTests
{
    private readonly BankPaymentRequest _validRequest = new()
    {
        CardNumber = "1234567890123456",
        ExpiryDate = "06/2027",
        Currency = "USD",
        Amount = 10000,
        Cvv = "123"
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private static BankClient CreateClient(HttpResponseMessage response)
    {
        var handler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://bank.com/") };
        return new BankClient(httpClient);
    }

    #region AuthorizePaymentAsync

    [Fact]
    public async Task AuthorizePaymentAsync_WhenBankAuthorizes_ReturnsAuthorizedResponse()
    {
        // Arrange
        var bankResponse = new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" };
        var sut = CreateClient(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(bankResponse, options: JsonOptions)
        });

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Authorized.ShouldBeTrue();
        result.AuthorizationCode.ShouldBe("AUTH123");
    }

    [Fact]
    public async Task AuthorizePaymentAsync_WhenBankDeclines_ReturnsDeclinedResponse()
    {
        // Arrange
        var bankResponse = new BankPaymentResponse { Authorized = false, AuthorizationCode = string.Empty };
        var sut = CreateClient(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(bankResponse, options: JsonOptions)
        });

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        result.ShouldNotBeNull();
        result.Authorized.ShouldBeFalse();
    }

    [Fact]
    public async Task AuthorizePaymentAsync_WhenBankReturnsNonSuccess_ReturnsNull()
    {
        // Arrange
        var sut = CreateClient(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task AuthorizePaymentAsync_WhenBankReturnsUnauthorized_ReturnsNull()
    {
        // Arrange
        var sut = CreateClient(new HttpResponseMessage(HttpStatusCode.Unauthorized));

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task AuthorizePaymentAsync_WhenBankReturnsNotFound_ReturnsNull()
    {
        // Arrange
        var sut = CreateClient(new HttpResponseMessage(HttpStatusCode.NotFound));

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task AuthorizePaymentAsync_WhenNetworkFails_ReturnsNull()
    {
        // Arrange
        var handler = new ThrowingHttpMessageHandler(new HttpRequestException("Network failure"));
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://bank.example.com/") };
        var sut = new BankClient(httpClient);

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task AuthorizePaymentAsync_WhenCancelled_ReturnsNull()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var handler = new ThrowingHttpMessageHandler(new TaskCanceledException());
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://bank.example.com/") };
        var sut = new BankClient(httpClient);
        await cts.CancelAsync();

        // Act
        var result = await sut.AuthorizePaymentAsync(_validRequest, cts.Token);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task AuthorizePaymentAsync_SendsRequestToCorrectEndpoint()
    {
        // Arrange
        var bankResponse = new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" };
        var handler = new CapturingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(bankResponse, options: JsonOptions)
        });
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://bank.example.com/") };
        var sut = new BankClient(httpClient);

        // Act
        await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        handler.CapturedRequest.ShouldNotBeNull();
        handler.CapturedRequest!.Method.ShouldBe(HttpMethod.Post);
        handler.CapturedRequest.RequestUri!.AbsolutePath.ShouldEndWith("payments");
    }

    [Fact]
    public async Task AuthorizePaymentAsync_SerializesRequestWithSnakeCase()
    {
        // Arrange
        var bankResponse = new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" };
        var handler = new CapturingHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(bankResponse, options: JsonOptions)
        });
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://bank.example.com/") };
        var sut = new BankClient(httpClient);

        // Act
        await sut.AuthorizePaymentAsync(_validRequest);

        // Assert
        var body = await handler.CapturedRequest!.Content!.ReadAsStringAsync();
        body.ShouldContain("card_number");
        body.ShouldContain("expiry_date");
    }

    #endregion
}