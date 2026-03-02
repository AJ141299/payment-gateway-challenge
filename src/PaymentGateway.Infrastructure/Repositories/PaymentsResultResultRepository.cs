using PaymentGateway.Core.Domain.Models;
using PaymentGateway.Core.Interfaces.Repositories;

namespace PaymentGateway.Infrastructure.Repositories;

public class PaymentsResultResultRepository : IPaymentsResultRepository
{
    public List<Payment> PaymentResults = new();
    
    public void Add(Payment payment)
    {
        PaymentResults.Add(payment);
    }
    
    public Payment? Get(string id)
    {
        return PaymentResults.FirstOrDefault(p => p.Id == id);
    }
}