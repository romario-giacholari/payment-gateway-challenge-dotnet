using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validation.Requests;

public class PostPaymentRequestValidator
{
    public List<string> Validate(PostPaymentRequest request)
    {
        var errors = new List<string>();
        
        return errors;
    }
}