using FluentValidation;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
{
    private static readonly string[] AllowedCurrencies = ["USD", "GBP", "PKR"];
    
    public ProcessPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithErrorCode("CARD_NUMBER_REQUIRED")
            .Length(14, 19).WithErrorCode("CARD_NUMBER_INVALID_LENGTH")
            .Matches(@"^\d+$").WithErrorCode("CARD_NUMBER_INVALID_FORMAT")
            .WithMessage("Card number must only contain numeric characters");
        
        RuleFor(x => x.ExpiryMonth)
            .NotEmpty().WithErrorCode("EXPIRY_MONTH_REQUIRED")
            .InclusiveBetween(1, 12).WithErrorCode("EXPIRY_MONTH_INVALID");

        RuleFor(x => x.ExpiryYear)
            .NotEmpty().WithErrorCode("EXPIRY_YEAR_REQUIRED")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year).WithErrorCode("EXPIRY_YEAR_INVALID")
            .WithMessage("Expiry year must be in future");
        
        RuleFor(x => x)
            .Must(x => IsExpiryInFuture(x.ExpiryMonth, x.ExpiryYear))
            .WithErrorCode("EXPIRY_DATE_IN_PAST")
            .WithMessage("Expiry date must be in future.")
            .WithName("Expiry");

        RuleFor(x => x.Currency)
            .NotEmpty().WithErrorCode("CURRENCY_REQUIRED")
            .Length(3).WithErrorCode("CURRENCY_INVALID_LENGTH")
            .Must(c => AllowedCurrencies.Contains(c?.ToUpper())).WithErrorCode("CURRENCY_NOT_SUPPORTED")
            .WithMessage($"Allowed currencies: {string.Join(", ", AllowedCurrencies)}.");

        RuleFor(x => x.Amount)
            .NotNull().WithErrorCode("AMOUNT_REQUIRED")
            .GreaterThanOrEqualTo(0ul).WithErrorCode("AMOUNT_INVALID");

        RuleFor(x => x.Cvv)
            .NotEmpty().WithErrorCode("CVV_REQUIRED")
            .Length(3, 4).WithErrorCode("CVV_INVALID_LENGTH")
            .Matches(@"^\d+$").WithErrorCode("CVV_INVALID_FORMAT")
            .WithMessage("CVV must only contain numeric characters");
    }

    private static bool IsExpiryInFuture(int month, int year)
    {
        var now = DateTime.UtcNow;
        return year > now.Year || (year == now.Year && month >= now.Month);
    }
}