namespace PaymentGateway.Api.Exceptions;

public class AcquiringBankUnavailableException(string message) : Exception(message);