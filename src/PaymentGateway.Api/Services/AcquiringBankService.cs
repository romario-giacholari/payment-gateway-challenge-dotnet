namespace PaymentGateway.Api.Services;

public class AcquiringBankService: IAcquiringBankService
{
    private readonly HttpClient _httpClient;

    public AcquiringBankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<object> ProcessPaymentAsync(object paymentRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("/payments", paymentRequest);

        if (!response.IsSuccessStatusCode)
        {
            return Task.FromResult((object)null);
        }
        
        return await response.Content.ReadFromJsonAsync<object>();
    }
}