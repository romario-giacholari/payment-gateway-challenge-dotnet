using PaymentGateway.Api.Models.Requests;

namespace PaymentGateway.Api.Validation.Requests;

public class PostPaymentRequestValidator
{
    public List<string> Validate(PostPaymentRequest request)
    {
        var errors = new List<string>();
        var allowedCurrencies = new HashSet<string>
        {
            "USD",
            "EUR",
            "GBP"
        };
        var currentYear = DateTime.Now.Year;
        var currentMonth = DateTime.Now.Month;

        if (request.ExpiryYear < currentYear)
        {
            errors.Add("Expiry year is earlier than current year");
        }

        if (request.ExpiryYear == currentYear && request.ExpiryMonth < currentMonth)
        {
            errors.Add("The card has expired");
        }

        if (!allowedCurrencies.Contains(request.Currency))
        {
            errors.Add("The currency is invalid");
        }
        
        return errors;
    }
}