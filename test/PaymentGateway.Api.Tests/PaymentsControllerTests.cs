using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Entities;
using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Repositories;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Tests;

public class PaymentsControllerTests
{
    private readonly Random _random = new();
    
    [Fact]
    public async Task RetrievesAPaymentSuccessfully()
    {
        // Arrange
        var payment = new PaymentEntity
        {
            Id = Guid.NewGuid(),
            ExpiryYear = _random.Next(2023, 2030),
            ExpiryMonth = _random.Next(1, 12),
            Amount = _random.Next(1, 10000),
            CardNumberLastFour = _random.Next(1111, 9999).ToString(),
            Currency = "GBP",
            Status = PaymentStatus.Authorized
        };

        var paymentsRepository = new PaymentsRepository();
        paymentsRepository.Add(payment);

        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services => ((ServiceCollection)services)
                .AddSingleton<IPaymentsRepository>(paymentsRepository)))
            .CreateClient();

        // Act
        var response = await client.GetAsync($"/api/Payments/{payment.Id}");
        var paymentResponse = await response.Content.ReadFromJsonAsync<GetPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(paymentResponse);
    }

    [Fact]
    public async Task Returns404IfPaymentNotFound()
    {
        // Arrange
        var webApplicationFactory = new WebApplicationFactory<PaymentsController>();
        var client = webApplicationFactory.CreateClient();
        
        // Act
        var response = await client.GetAsync($"/api/Payments/{Guid.NewGuid()}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ProcessesAPaymentSuccessfully()
    {
        // Arrange
        var mockAcquiringBankService = new Mock<IAcquiringBankService>();
        mockAcquiringBankService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<AcquiringBankPostPaymentRequest>()))
            .ReturnsAsync(new AcquiringBankResponse
            {
                Authorized = true,
                AuthorizationCode = Guid.NewGuid().ToString()
            });
        
        var client = CreateClientWithMockAcquiringBankService(mockAcquiringBankService);
        // authorized response when the last digit falls in 1, 3, 5, 7, 9
        var request = CreateStubPostPaymentRequest("4242424242424241");
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);
        var result = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("Authorized", result?.Status);
    }
    
    [Fact]
    public async Task TheAcquiringBankReturnsDeclined()
    {
        // Arrange
        var mockAcquiringBankService = new Mock<IAcquiringBankService>();
        mockAcquiringBankService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<AcquiringBankPostPaymentRequest>()))
            .ReturnsAsync(new AcquiringBankResponse
            {
                Authorized = false,
                AuthorizationCode = ""
            });
        
        var client = CreateClientWithMockAcquiringBankService(mockAcquiringBankService);
        
        // unauthorized response when the last digit falls in 2, 4, 6, 8
        var request = CreateStubPostPaymentRequest("4242424242424242");
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);
        var result = await response.Content.ReadFromJsonAsync<PostPaymentResponse>();
       
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal("Declined", result?.Status);
    }
    
    [Fact]
    public async Task AcquiringBankIsUnavailable()
    {
        // Arrange
        var mockAcquiringBankService = new Mock<IAcquiringBankService>();
        mockAcquiringBankService
            .Setup(x => x.ProcessPaymentAsync(It.IsAny<AcquiringBankPostPaymentRequest>()))
            .Throws(new AcquiringBankUnavailableException("The acquiring bank service is unavailable"));
        
        var client = CreateClientWithMockAcquiringBankService(mockAcquiringBankService);
        
        // unavailable response when the last digit is 0
        var request = CreateStubPostPaymentRequest("4242424242424240");
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);
        var content = await response.Content.ReadAsStringAsync();
        
        // Assert
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Contains(AcquiringBankUnavailableException.AcquiringBankUnavailableMessage, content);
    }

    [Fact]
    public async Task EnsureValidation()
    {
        // Arrange
        var factory = new WebApplicationFactory<PaymentsController>();
        var client = factory.CreateClient();
        var request = new PostPaymentRequest
        {
            CardNumber = "test",
            ExpiryMonth = -1,
            ExpiryYear = 2020,
            Currency = "test",
            Amount = 0,
            Cvv = "1"
        };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/Payments", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private HttpClient CreateClientWithMockAcquiringBankService(Mock<IAcquiringBankService> mockAcquiringBankService)
    {
        var factory = new WebApplicationFactory<PaymentsController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IAcquiringBankService));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddSingleton(mockAcquiringBankService.Object);
                });
            });

        return factory.CreateClient();
    }
    
    private PostPaymentRequest CreateStubPostPaymentRequest(string cardNumber = "4242424242424243")
    {
        return new PostPaymentRequest
        {
            CardNumber = cardNumber,
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Currency = "GBP",
            Amount = 1050,
            Cvv = "123"
        };
    }
}