using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Core.Domain.Models;
using PaymentGateway.IntegrationTests.Hooks;
using Reqnroll;
using Shouldly;

namespace PaymentGateway.IntegrationTests.Steps;

[Binding]
public class GetPaymentSteps
{
    private readonly ScenarioContext _scenarioContext;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public GetPaymentSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [Given("a payment exists with id {string}")]
    public void GivenAPaymentExistsWithId(string id)
    {
        var factory = _scenarioContext.Get<TestWebApplicationFactory>("Factory");

        var payment = new Payment
        {
            Id = id,
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "3456",
            ExpiryMonth = 6,
            ExpiryYear = 2027,
            Currency = "USD",
            Amount = 10000
        };

        factory.PaymentsRepository.PaymentResults.Add(payment);
        _scenarioContext["SeedPayment"] = payment;
    }

    [Given("no payment exists with id {string}")]
    public void GivenNoPaymentExistsWithId(string id)
    {
    }

    [When("I request the payment with id {string}")]
    public async Task WhenIRequestThePaymentWithId(string id)
    {
        var client = _scenarioContext.Get<HttpClient>("HttpClient");
        var response = await client.GetAsync($"/api/v1/payments/{id}");
        _scenarioContext["Response"] = response;
    }

    [Then("the response status should be {int}")]
    public void ThenTheResponseStatusShouldBe(int expectedStatus)
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("Response");
        ((int)response.StatusCode).ShouldBe(expectedStatus);
    }

    [Then("the response should contain the payment details")]
    public async Task ThenTheResponseShouldContainThePaymentDetails()
    {
        var response = _scenarioContext.Get<HttpResponseMessage>("Response");
        var seededPayment = _scenarioContext.Get<Payment>("SeedPayment");

        var body = await response.Content.ReadFromJsonAsync<GetPaymentResponse>(JsonOptions);

        body.ShouldNotBeNull();
        body.Id.ShouldBe(seededPayment.Id);
        body.Status.ShouldBe(seededPayment.Status);
        body.CardNumberLastFour.ShouldBe(seededPayment.CardNumberLastFour);
        body.ExpiryMonth.ShouldBe(seededPayment.ExpiryMonth);
        body.ExpiryYear.ShouldBe(seededPayment.ExpiryYear);
        body.Currency.ShouldBe(seededPayment.Currency);
        body.Amount.ShouldBe(seededPayment.Amount);
    }
}