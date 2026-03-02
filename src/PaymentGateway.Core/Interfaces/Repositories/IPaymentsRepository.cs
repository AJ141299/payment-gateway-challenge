using PaymentGateway.Core.Domain.Models;

namespace PaymentGateway.Core.Interfaces.Repositories;

public interface IPaymentsRepository
{
    void Add(Payment payment);
    Payment? Get(string id);
}