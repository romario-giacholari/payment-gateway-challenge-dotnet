using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public interface IPaymentService
{
    GetPaymentResponse? GetPayment(Guid id);

    Task<(PostPaymentResponse? Response, IReadOnlyList<string>? Errors)> ProcessPaymentAsync(
        PostPaymentRequest paymentRequest);
}