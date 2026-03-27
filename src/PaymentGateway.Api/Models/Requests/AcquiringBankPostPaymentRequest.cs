namespace PaymentGateway.Api.Models.Requests;

public class AcquiringBankPostPaymentRequest
{
    public string card_number { get; set; }
    public string expiry_date { get; set; }
    public string currency { get; set; }
    public int amount { get; set; }
    public string cvv { get; set; }
}