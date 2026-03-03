using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Core.Domain.Models;
using PaymentGateway.Core.Services;
using Shouldly;

namespace PaymentGateway.Api.Tests.Controllers;

public class PaymentsControllerTests
{
    private readonly IPaymentsService _paymentsService;
    private readonly IValidator<ProcessPaymentRequest> _validator;
    private readonly PaymentsController _sut;

    private static readonly ProcessPaymentRequest ValidRequest = new()
    {
        CardNumber = "1234567890123456",
        ExpiryMonth = 6,
        ExpiryYear = 2027,
        Currency = "USD",
        Amount = 10000,
        Cvv = "123"
    };

    private static readonly Payment ValidPayment = new()
    {
        Id = "payment-id",
        Status = PaymentStatus.Authorized,
        CardNumberLastFour = "3456",
        ExpiryMonth = 6,
        ExpiryYear = 2027,
        Currency = "USD",
        Amount = 10000
    };

    public PaymentsControllerTests()
    {
        _paymentsService = Substitute.For<IPaymentsService>();
        _validator = Substitute.For<IValidator<ProcessPaymentRequest>>();
        _sut = new PaymentsController(_paymentsService, _validator);
    }

    #region ProcessPaymentAsync

    [Fact]
    public async Task ProcessPaymentAsync_WhenRequestIsValid_Returns200()
    {
        // Arrange
        _validator.ValidateAsync(ValidRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        _paymentsService.ProcessPaymentAsync(Arg.Any<PaymentDetails>(), Arg.Any<CancellationToken>())
            .Returns(ValidPayment);

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidRequest, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<OkObjectResult>()
            .StatusCode.ShouldBe(200);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenRequestIsValid_ReturnsProcessPaymentResponse()
    {
        // Arrange
        _validator.ValidateAsync(ValidRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());
        _paymentsService.ProcessPaymentAsync(Arg.Any<PaymentDetails>(), Arg.Any<CancellationToken>())
            .Returns(ValidPayment);

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidRequest, CancellationToken.None);

        // Assert
        var response = result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<ProcessPaymentResponse>();
        response.Id.ShouldBe(ValidPayment.Id);
        response.Status.ShouldBe(ValidPayment.Status);
        response.CardNumberLastFour.ShouldBe(ValidPayment.CardNumberLastFour);
        response.Amount.ShouldBe(ValidPayment.Amount);
        response.Currency.ShouldBe(ValidPayment.Currency);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenValidationFails_Returns400()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("CardNumber", "Card number is required") { ErrorCode = "NotEmptyValidator" }
        };
        _validator.ValidateAsync(ValidRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(failures));

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidRequest, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>()
            .StatusCode.ShouldBe(400);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenValidationFails_DoesNotCallService()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("CardNumber", "Card number is required") { ErrorCode = "NotEmptyValidator" }
        };
        _validator.ValidateAsync(ValidRequest, Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(failures));

        // Act
        await _sut.ProcessPaymentAsync(ValidRequest, CancellationToken.None);

        // Assert
        await _paymentsService.DidNotReceive()
            .ProcessPaymentAsync(Arg.Any<PaymentDetails>(), Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetPaymentAsync

    [Fact]
    public void GetPaymentAsync_WhenPaymentExists_Returns200()
    {
        // Arrange
        _paymentsService.GetPayment(ValidPayment.Id).Returns(ValidPayment);

        // Act
        var result = _sut.GetPaymentAsync(ValidPayment.Id);

        // Assert
        result.ShouldBeOfType<OkObjectResult>()
            .StatusCode.ShouldBe(200);
    }

    [Fact]
    public void GetPaymentAsync_WhenPaymentExists_ReturnsGetPaymentResponse()
    {
        // Arrange
        _paymentsService.GetPayment(ValidPayment.Id).Returns(ValidPayment);

        // Act
        var result = _sut.GetPaymentAsync(ValidPayment.Id);

        // Assert
        var response = result.ShouldBeOfType<OkObjectResult>().Value.ShouldBeOfType<GetPaymentResponse>();
        response.Id.ShouldBe(ValidPayment.Id);
        response.Status.ShouldBe(ValidPayment.Status);
        response.CardNumberLastFour.ShouldBe(ValidPayment.CardNumberLastFour);
        response.Amount.ShouldBe(ValidPayment.Amount);
        response.Currency.ShouldBe(ValidPayment.Currency);
    }

    [Fact]
    public void GetPaymentAsync_WhenPaymentDoesNotExist_Returns404()
    {
        // Arrange
        _paymentsService.GetPayment(Arg.Any<string>()).Returns((Payment?)null);

        // Act
        var result = _sut.GetPaymentAsync("nonexistent-id");

        // Assert
        result.ShouldBeOfType<NotFoundResult>()
            .StatusCode.ShouldBe(404);
    }

    #endregion
}