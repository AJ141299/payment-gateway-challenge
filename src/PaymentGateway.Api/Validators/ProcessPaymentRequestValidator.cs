using FluentValidation;
using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validators;

public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
{
    private static readonly string[] AllowedCurrencies = ["USD", "GBP", "PKR"];
    
    public ProcessPaymentRequestValidator()
    {
        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .Length(14, 19)
            .Matches(@"^\d+$")
            .WithMessage("Card number must only contain numeric characters");
        
        RuleFor(x => x.ExpiryMonth)
            .NotEmpty()
            .InclusiveBetween(1, 12);

        RuleFor(x => x.ExpiryYear)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow.Year)
            .WithMessage("Expiry year must be in future");
        
        RuleFor(x => x)
            .Must(x => IsExpiryInFuture(x.ExpiryMonth, x.ExpiryYear))
            .WithMessage("Expiry date must be in future.")
            .WithName("Expiry");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3)
            .Must(c => AllowedCurrencies.Contains(c?.ToUpper()))
            .WithMessage($"Allowed currencies: {string.Join(", ", AllowedCurrencies)}.");

        RuleFor(x => x.Amount)
            .NotNull()
            .GreaterThanOrEqualTo(0ul);

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .Length(3, 4)
            .Matches(@"^\d+$")
            .WithMessage("CVV must only contain numeric characters");
    }

    private static bool IsExpiryInFuture(int month, int year)
    {
        var now = DateTime.UtcNow;
        return year > now.Year || (year == now.Year && month >= now.Month);
    }
}