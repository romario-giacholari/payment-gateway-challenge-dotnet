namespace PaymentGateway.Api.Services;

public interface IAcquiringBankService
{
    Task<object> ProcessPaymentAsync(object paymentRequest);
}