using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Entities;

public class PaymentEntity
{ 
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string CardNumberLastFour { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }

    public GetPaymentResponse ToGetPaymentResponse()
    {
        return new GetPaymentResponse
        {
            Id = Id,
            Status = Status.ToString(),
            CardNumberLastFour = CardNumberLastFour,
            ExpiryMonth = ExpiryMonth,
            ExpiryYear = ExpiryYear,
            Currency = Currency,
            Amount = Amount
        };
    }
}