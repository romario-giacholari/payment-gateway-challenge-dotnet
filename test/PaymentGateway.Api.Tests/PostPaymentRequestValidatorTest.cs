using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Validation.Requests;

namespace PaymentGateway.Api.Tests;

public class PostPaymentRequestValidatorTest
{
    private readonly PostPaymentRequestValidator _validator = new();
    
    [Theory]
    [InlineData(12, 1999, "GBP", "Expiry year is earlier than current year")] // very old card
    [InlineData(1, 2026, "GBP", "The card has expired")]   // expired card
    [InlineData(1, 2028, "XYZ", "The currency is invalid")]   // invalid currency
    public async Task ValidatesTheRequest(int expiryMonth, int expiryYear, string currency, string error)
    {
        // Arrange
        var request = new PostPaymentRequest
        {
            CardNumber = "4242424242424243",
            ExpiryMonth = expiryMonth,
            ExpiryYear = expiryYear,
            Currency = currency,
            Amount = 1,
            Cvv = "123"
        };
        
        // Act
        var errors = _validator.Validate(request);
        
        // Assert
        Assert.NotEmpty(errors);
        Assert.Contains(errors, e => e == error);
    }
}