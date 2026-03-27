using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.Repositories;

public interface IPaymentsRepository
{
    void Add(PaymentEntity payment);
    PaymentEntity? Get(Guid id);
}