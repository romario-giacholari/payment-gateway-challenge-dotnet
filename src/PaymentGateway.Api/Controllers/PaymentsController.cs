using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Exceptions;
using PaymentGateway.Api.Models;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<GetPaymentResponse?>> GetPaymentAsync(Guid id)
    {
        var payment = _paymentService.GetPayment(id);

        if (payment == null)
        {
            return NotFound();
        }

        return Ok(payment);
    }
    
    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse?>> PostPaymentAsync([FromBody]PostPaymentRequest paymentRequest)
    {
        (PostPaymentResponse? postPaymentResponse, IReadOnlyList<string>? errors) = await _paymentService.ProcessPaymentAsync(paymentRequest);

        if (errors == null)
        {
            return Ok(postPaymentResponse);
        }

        if (errors.Any(x => x == AcquiringBankUnavailableException.AcquiringBankUnavailableMessage))
        {
            return Ok(errors);
        }
            
        return BadRequest(errors);

    }
}