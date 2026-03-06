using PaymentGateway.Core.Domain.Enums;
using PaymentGateway.Core.Domain.Models;
using PaymentGateway.Core.Exceptions;
using PaymentGateway.Core.Interfaces.Clients;
using PaymentGateway.Core.Interfaces.Clients.Models;
using PaymentGateway.Core.Interfaces.Repositories;

namespace PaymentGateway.Core.Services;

public class PaymentsService(IBankClient bankClient, IPaymentsRepository paymentsRepository) : IPaymentsService
{
    public async Task<Payment> ProcessPaymentAsync(PaymentDetails paymentDetails, CancellationToken ct = default)
    {
        // TODO: does a payment exist for this request Id?
        
        var paymentId = Guid.NewGuid().ToString();
        
        var bankRequest = BankPaymentRequest.FromDomain(paymentDetails);
        var bankResponse = await bankClient.AuthorizePaymentAsync(bankRequest, ct);
        if (bankResponse == null)
        {
            throw new PaymentProcessingException();
        }
        
        var result = new Payment
        {
            Id = paymentId,
            Status = bankResponse.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
            CardNumberLastFour = paymentDetails.CardNumber[^4..],
            ExpiryMonth = paymentDetails.ExpiryMonth,
            ExpiryYear = paymentDetails.ExpiryYear,
            Currency = paymentDetails.Currency,
            Amount = paymentDetails.Amount
        };
        
        paymentsRepository.Add(result);
        
        // TODO: store requestId against paymentId

        return result;
    }
    
    public Payment? GetPayment(string id)
    {
        return paymentsRepository.Get(id);
    }
}