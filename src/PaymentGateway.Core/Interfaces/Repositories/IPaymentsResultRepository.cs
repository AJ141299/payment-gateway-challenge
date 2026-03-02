using PaymentGateway.Core.Domain.Models;

namespace PaymentGateway.Core.Interfaces.Repositories;

public interface IPaymentsResultRepository
{
    void Add(Payment payment);
    Payment? Get(string id);
}