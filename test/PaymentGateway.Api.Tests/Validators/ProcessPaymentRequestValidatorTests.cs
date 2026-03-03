using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validators;
using Shouldly;

namespace PaymentGateway.Api.Tests.Validators;

public class ProcessPaymentRequestValidatorTests
{
    private readonly ProcessPaymentRequestValidator _sut = new();

    private static ProcessPaymentRequest ValidRequest() => new()
    {
        CardNumber = "1234567890123456",
        ExpiryMonth = 6,
        ExpiryYear = DateTime.UtcNow.Year + 1,
        Currency = "USD",
        Amount = 10000,
        Cvv = "123"
    };

    #region CardNumber

    [Fact]
    public async Task Validate_WhenCardNumberIsEmpty_ReturnsCardNumberRequiredError()
    {
        var request = ValidRequest();
        request.CardNumber = "";

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CARD_NUMBER_REQUIRED");
    }

    [Theory]
    [InlineData("1234567890123")] // 13 digits - too short
    [InlineData("12345678901234567890")] // 20 digits - too long
    public async Task Validate_WhenCardNumberLengthIsInvalid_ReturnsCardNumberInvalidLengthError(string cardNumber)
    {
        var request = ValidRequest();
        request.CardNumber = cardNumber;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CARD_NUMBER_INVALID_LENGTH");
    }

    [Fact]
    public async Task Validate_WhenCardNumberContainsNonNumericCharacters_ReturnsCardNumberInvalidFormatError()
    {
        var request = ValidRequest();
        request.CardNumber = "abcd1234abcd1234";

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CARD_NUMBER_INVALID_FORMAT");
    }

    #endregion

    #region ExpiryMonth

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public async Task Validate_WhenExpiryMonthIsOutOfRange_ReturnsExpiryMonthInvalidError(int month)
    {
        var request = ValidRequest();
        request.ExpiryMonth = month;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "EXPIRY_MONTH_INVALID");
    }

    #endregion

    #region ExpiryYear

    [Fact]
    public async Task Validate_WhenExpiryYearIsInPast_ReturnsExpiryYearInvalidError()
    {
        var request = ValidRequest();
        request.ExpiryYear = DateTime.UtcNow.Year - 1;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "EXPIRY_YEAR_INVALID");
    }

    #endregion

    #region Expiry Date (combined)

    [Fact]
    public async Task Validate_WhenExpiryMonthIsInPastForCurrentYear_ReturnsExpiryDateInPastError()
    {
        var request = ValidRequest();
        request.ExpiryMonth = DateTime.UtcNow.Month - 1 < 1 ? 1 : DateTime.UtcNow.Month - 1;
        request.ExpiryYear = DateTime.UtcNow.Year;

        // Only relevant if we're not in January - skip if no previous month exists this year
        if (DateTime.UtcNow.Month == 1) return;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "EXPIRY_DATE_IN_PAST");
    }

    [Fact]
    public async Task Validate_WhenExpiryMonthIsCurrentMonthAndYear_IsValid()
    {
        var request = ValidRequest();
        request.ExpiryMonth = DateTime.UtcNow.Month;
        request.ExpiryYear = DateTime.UtcNow.Year;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeTrue();
    }

    #endregion

    #region Currency

    [Fact]
    public async Task Validate_WhenCurrencyIsEmpty_ReturnsCurrencyRequiredError()
    {
        var request = ValidRequest();
        request.Currency = "";

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CURRENCY_REQUIRED");
    }

    [Fact]
    public async Task Validate_WhenCurrencyIsNotThreeCharacters_ReturnsCurrencyInvalidLengthError()
    {
        var request = ValidRequest();
        request.Currency = "US";

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CURRENCY_INVALID_LENGTH");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("JPY")]
    [InlineData("AUD")]
    public async Task Validate_WhenCurrencyIsNotSupported_ReturnsCurrencyNotSupportedError(string currency)
    {
        var request = ValidRequest();
        request.Currency = currency;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CURRENCY_NOT_SUPPORTED");
    }

    [Theory]
    [InlineData("USD")]
    [InlineData("GBP")]
    [InlineData("PKR")]
    public async Task Validate_WhenCurrencyIsSupported_IsValid(string currency)
    {
        var request = ValidRequest();
        request.Currency = currency;

        var result = await _sut.ValidateAsync(request);

        result.Errors.ShouldNotContain(e => e.ErrorCode == "CURRENCY_NOT_SUPPORTED");
    }

    #endregion

    #region CVV

    [Fact]
    public async Task Validate_WhenCvvIsEmpty_ReturnsCvvRequiredError()
    {
        var request = ValidRequest();
        request.Cvv = "";

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CVV_REQUIRED");
    }

    [Theory]
    [InlineData("12")]    // too short
    [InlineData("12345")] // too long
    public async Task Validate_WhenCvvLengthIsInvalid_ReturnsCvvInvalidLengthError(string cvv)
    {
        var request = ValidRequest();
        request.Cvv = cvv;

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CVV_INVALID_LENGTH");
    }

    [Fact]
    public async Task Validate_WhenCvvContainsNonNumericCharacters_ReturnsCvvInvalidFormatError()
    {
        var request = ValidRequest();
        request.Cvv = "ab3";

        var result = await _sut.ValidateAsync(request);

        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.ErrorCode == "CVV_INVALID_FORMAT");
    }

    [Theory]
    [InlineData("123")]  // 3 digits
    [InlineData("1234")] // 4 digits
    public async Task Validate_WhenCvvIsValid_IsValid(string cvv)
    {
        var request = ValidRequest();
        request.Cvv = cvv;

        var result = await _sut.ValidateAsync(request);

        result.Errors.ShouldNotContain(e => e.ErrorCode == "CVV_INVALID_LENGTH" || e.ErrorCode == "CVV_INVALID_FORMAT");
    }

    #endregion

    #region Valid Request

    [Fact]
    public async Task Validate_WhenRequestIsValid_ReturnsNoErrors()
    {
        var result = await _sut.ValidateAsync(ValidRequest());

        result.IsValid.ShouldBeTrue();
    }

    #endregion
}