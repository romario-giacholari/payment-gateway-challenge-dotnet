using PaymentGateway.Api.Models.Responses;
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

    public GetPaymentResponse? GetPaymentAsync(Guid id)
    { 
        var payment = _paymentsRepository.Get(id);

        return payment?.ToGetPaymentResponse();
    }

    public PostPaymentResponse ProcessPaymentAsync()
    {
        var postPaymentResponse = new PostPaymentResponse();
        return postPaymentResponse;
    }
}