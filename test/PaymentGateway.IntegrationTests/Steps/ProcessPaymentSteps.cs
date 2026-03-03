using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.IntegrationTests.Hooks;

using Reqnroll;
using Shouldly;

namespace PaymentGateway.IntegrationTests.Steps;

[Binding]
public class ProcessPaymentSteps
{
    private readonly ScenarioContext _scenarioContext;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly Dictionary<string, object> BaseRequest = new()
    {
        ["cardNumber"] = "1234567890123456",
        ["expiryMonth"] = 6,
        ["expiryYear"] = DateTime.UtcNow.Year + 1,
        ["currency"] = "USD",
        ["amount"] = 10000,
        ["cvv"] = "123"
    };

    public ProcessPaymentSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("the bank will authorize the payment")]
    public void GivenTheBankWillAuthorizeThePayment()
    {
        var factory = _scenarioContext.Get<TestWebApplicationFactory>("Factory");
        factory.BankHandler.RespondWith(authorized: true, authCode: "AUTH123");
    }

    [Given("the bank will decline the payment")]
    public void GivenTheBankWillDeclineThePayment()
    {
        var factory = _scenarioContext.Get<TestWebApplicationFactory>("Factory");
        factory.BankHandler.RespondWith(authorized: false, authCode: string.Empty);
    }

    [When("I submit a valid payment request")]
    public async Task WhenISubmitAValidPaymentRequest()
    {
        var client = _scenarioContext.Get<HttpClient>("HttpClient");
        var response = await client.PostAsJsonAsync("/api/v1/payments/process", BaseRequest, JsonOptions);
        _scenarioContext["Response"] = response;
    }

    [Then("the response should contain an authorized payment")]
    public async Task ThenTheResponseShouldContainAnAuthorizedPayment()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("Response");
        var body = await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>(JsonOptions);

        body.ShouldNotBeNull();
        body.Status.ShouldBe(PaymentStatus.Authorized);
        body.Id.ShouldNotBeNullOrEmpty();
        body.CardNumberLastFour.ShouldBe("3456");
    }

    [Then("the response should contain a declined payment")]
    public async Task ThenTheResponseShouldContainADeclinedPayment()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("Response");
        var body = await response.Content.ReadFromJsonAsync<ProcessPaymentResponse>(JsonOptions);

        body.ShouldNotBeNull();
        body.Status.ShouldBe(PaymentStatus.Declined);
    }
}