using PaymentGateway.Api.Repositories;

namespace PaymentGateway.Api.Services;

public class PaymentService
{
    private readonly PaymentsRepository _paymentsRepository;
    private readonly IAcquiringBankService _acquiringBankService;
    
    public PaymentService(PaymentsRepository paymentRepository, IAcquiringBankService acquiringBankService)
    {
        _paymentsRepository = paymentRepository;
        _acquiringBankService = acquiringBankService;
    }

    public async Task<object> getPayment(Guid id)
    {
       return _paymentsRepository.Get(id);
    }

    public async Task<object> processPayment()
    {
        return new { };
    }
}