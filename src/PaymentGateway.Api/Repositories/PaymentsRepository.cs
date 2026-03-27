using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.Repositories;

public class PaymentsRepository: IPaymentsRepository
{
    private readonly List<PaymentEntity> _payments = [];
    
    public void Add(PaymentEntity payment)
    {
        if (!_payments.Contains(payment))
        {
            _payments.Add(payment);
        }
    }

    public PaymentEntity? Get(Guid id)
    {
        return _payments.FirstOrDefault(p => p.Id == id);
    }
}