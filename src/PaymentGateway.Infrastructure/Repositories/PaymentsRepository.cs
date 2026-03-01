using PaymentGateway.Core.Interfaces;
using PaymentGateway.Core.Interfaces.Repositories;

namespace PaymentGateway.Infrastructure.Repositories;

public class PaymentsRepository : IPaymentsRepository
{
    // public List<PostPaymentResponse> Payments = new();
    //
    // public void Add(PostPaymentResponse payment)
    // {
    //     Payments.Add(payment);
    // }
    //
    // public PostPaymentResponse Get(Guid id)
    // {
    //     return Payments.FirstOrDefault(p => p.Id == id);
    // }
}