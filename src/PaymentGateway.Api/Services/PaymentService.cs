using PaymentGateway.Api.Entities;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Validation.Requests;

namespace PaymentGateway.Api.Services;

public class PaymentService
{
    private readonly PaymentsRepository _paymentsRepository;
    private readonly IAcquiringBankService _acquiringBankService;
    private readonly PostPaymentRequestValidator _postPaymentRequestValidator;
    
    public PaymentService(PaymentsRepository paymentRepository, IAcquiringBankService acquiringBankService, PostPaymentRequestValidator postPaymentRequestValidator)
    {
        _paymentsRepository = paymentRepository;
        _acquiringBankService = acquiringBankService;
        _postPaymentRequestValidator = postPaymentRequestValidator;
    }

    public GetPaymentResponse? GetPaymentAsync(Guid id)
    { 
        var payment = _paymentsRepository.Get(id);

        return payment?.ToGetPaymentResponse();
    }

    public async Task<(PostPaymentResponse? Response, IReadOnlyList<string>? Errors)> ProcessPaymentAsync(PostPaymentRequest paymentRequest)
    {
        var errors = _postPaymentRequestValidator.Validate(paymentRequest);
        if (errors.Count > 0)
        {
            return (null, errors);
        }
        
        var acquiringBankResponse = await _acquiringBankService.ProcessPaymentAsync(new AcquiringBankPostPaymentRequest
        {
            card_number = paymentRequest.CardNumber,
            expiry_date = $"{paymentRequest.ExpiryMonth:D2}/{paymentRequest.ExpiryYear}",
            currency =  paymentRequest.Currency.ToUpper(),
            amount = paymentRequest.Amount,
            cvv = paymentRequest.Cvv,
        });

        var authorized = acquiringBankResponse?.Authorized ?? false;
        var status = authorized ? PaymentStatus.Authorized : PaymentStatus.Declined;
        var isGuidParsed = Guid.TryParse(acquiringBankResponse?.AuthorizationCode, out var code);
        var id = isGuidParsed ? code : Guid.NewGuid();
        
        _paymentsRepository.Add(new PaymentEntity
        {
            Id = id,
            Status = status,
            CardNumberLastFour = paymentRequest.CardNumber[^4..],
            ExpiryMonth = paymentRequest.ExpiryMonth,
            ExpiryYear = paymentRequest.ExpiryYear,
            Currency = paymentRequest.Currency.ToUpper(),
            Amount = paymentRequest.Amount
        });

        return (new PostPaymentResponse
        {
            Id = id,
            Status = status.ToString(),
            CardNumberLastFour = paymentRequest.CardNumber[^4..],
            ExpiryMonth = paymentRequest.ExpiryMonth,
            ExpiryYear = paymentRequest.ExpiryYear,
            Currency = paymentRequest.Currency.ToUpper(),
            Amount = paymentRequest.Amount
        }, null);
    }
}