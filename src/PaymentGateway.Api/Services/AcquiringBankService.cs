using System.Text.Json;

using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;

namespace PaymentGateway.Api.Services;

public class AcquiringBankService: IAcquiringBankService
{
    private readonly HttpClient _httpClient;

    public AcquiringBankService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<AcquiringBankResponse?> ProcessPaymentAsync(AcquiringBankPostPaymentRequest paymentRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("/payments", paymentRequest);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        
        return JsonSerializer.Deserialize<AcquiringBankResponse>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}