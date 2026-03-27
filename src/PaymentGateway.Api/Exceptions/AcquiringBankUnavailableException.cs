namespace PaymentGateway.Api.Exceptions;

public class AcquiringBankUnavailableException(string message) : Exception(message)
{
    public const string AcquiringBankUnavailableMessage = "The service is unavailable right now. No money was taken from your card. Please try again later";
}