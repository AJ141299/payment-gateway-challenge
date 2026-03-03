using NSubstitute;
using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Core.Domain.Models;
using PaymentGateway.Core.Exceptions;
using PaymentGateway.Core.Interfaces.Clients;
using PaymentGateway.Core.Interfaces.Clients.Models;
using PaymentGateway.Core.Interfaces.Repositories;
using PaymentGateway.Core.Services;
using Shouldly;

namespace PaymentGateway.Core.Tests.Services;

public class PaymentsServiceTests
{
    private readonly IBankClient _bankClient;
    private readonly IPaymentsRepository _paymentsRepository;
    private readonly PaymentsService _sut;
    private static readonly PaymentDetails ValidPaymentDetails = new()
    {
        CardNumber = "1234567890123456",
        ExpiryMonth = 6,
        ExpiryYear = 2027,
        Currency = "USD",
        Amount = 10000,
        Cvv = "123"
    };

    public PaymentsServiceTests()
    {
        _bankClient = Substitute.For<IBankClient>();
        _paymentsRepository = Substitute.For<IPaymentsRepository>();
        _sut = new PaymentsService(_bankClient, _paymentsRepository);
    }

    #region ProcessPaymentAsync

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankReturnsNull_ThrowsPaymentProcessingException()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns((BankPaymentResponse?)null);

        // Act & Assert
        await Should.ThrowAsync<PaymentProcessingException>(() => _sut.ProcessPaymentAsync(ValidPaymentDetails));
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankReturnsNull_DoesNotAddPaymentToRepository()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns((BankPaymentResponse?)null);

        // Act
        try { await _sut.ProcessPaymentAsync(ValidPaymentDetails); } catch (PaymentProcessingException) { }

        // assert
        _paymentsRepository.DidNotReceive().Add(Arg.Any<Payment>());
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankAuthorizes_ReturnsAuthorizedPayment()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" });

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidPaymentDetails);

        // Assert
        result.Status.ShouldBe(PaymentStatus.Authorized);
    }

    [Fact]
    public async Task ProcessPaymentAsync_WhenBankDoesNotAuthorize_ReturnsDeclinedPayment()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new BankPaymentResponse { Authorized = false, AuthorizationCode = string.Empty });

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidPaymentDetails);

        // Assert
        result.Status.ShouldBe(PaymentStatus.Declined);
    }

    [Fact]
    public async Task ProcessPaymentAsync_ReturnsPaymentWithCorrectDetails()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" });

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidPaymentDetails);

        // Assert
        result.CardNumberLastFour.ShouldBe("3456");
        result.ExpiryMonth.ShouldBe(ValidPaymentDetails.ExpiryMonth);
        result.ExpiryYear.ShouldBe(ValidPaymentDetails.ExpiryYear);
        result.Currency.ShouldBe(ValidPaymentDetails.Currency);
        result.Amount.ShouldBe(ValidPaymentDetails.Amount);
    }

    [Fact]
    public async Task ProcessPaymentAsync_AddsPaymentToRepository()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" });

        // Act
        var result = await _sut.ProcessPaymentAsync(ValidPaymentDetails);

        // Assert
        _paymentsRepository.Received(1).Add(Arg.Is<Payment>(p => p.Id == result.Id));
    }

    [Fact]
    public async Task ProcessPaymentAsync_SendsCorrectRequestToBank()
    {
        // Arrange
        _bankClient.AuthorizePaymentAsync(Arg.Any<BankPaymentRequest>(), Arg.Any<CancellationToken>())
            .Returns(new BankPaymentResponse { Authorized = true, AuthorizationCode = "AUTH123" });

        // Act
        await _sut.ProcessPaymentAsync(ValidPaymentDetails);

        // Assert
        await _bankClient.Received(1).AuthorizePaymentAsync(
            Arg.Is<BankPaymentRequest>(r =>
                r.CardNumber == ValidPaymentDetails.CardNumber &&
                r.Currency == ValidPaymentDetails.Currency &&
                r.Amount == ValidPaymentDetails.Amount &&
                r.Cvv == ValidPaymentDetails.Cvv &&
                r.ExpiryDate == $"{ValidPaymentDetails.ExpiryMonth:D2}/{ValidPaymentDetails.ExpiryYear}"),
            Arg.Any<CancellationToken>());
    }

    #endregion

    #region GetPayment

    [Fact]
    public void GetPayment_WhenPaymentExists_ReturnsPayment()
    {
        // Arrange
        var payment = new Payment
        {
            Id = "test-id",
            Status = PaymentStatus.Authorized,
            CardNumberLastFour = "3456",
            ExpiryMonth = 6,
            ExpiryYear = 2027,
            Currency = "USD",
            Amount = 10000
        };
        _paymentsRepository.Get("test-id").Returns(payment);

        // Act
        var result = _sut.GetPayment("test-id");

        // Assert
        result.ShouldBe(payment);
    }

    [Fact]
    public void GetPayment_WhenPaymentDoesNotExist_ReturnsNull()
    {
        // Arrange
        _paymentsRepository.Get(Arg.Any<string>()).Returns((Payment?)null);

        // Act
        var result = _sut.GetPayment("nonexistent-id");

        // Assert
        result.ShouldBeNull();
    }

    #endregion
}