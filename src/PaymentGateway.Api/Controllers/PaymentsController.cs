using FluentValidation;

using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Core.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/v1/payments")]
[ApiController]
public class PaymentsController(
    IPaymentsService paymentsService,
    IValidator<ProcessPaymentRequest> processPaymentValidator) : ControllerBase
{
    [HttpPost("process")]
    public async Task<ActionResult> ProcessPaymentAsync(ProcessPaymentRequest request, CancellationToken ct)
    {
        var result = await processPaymentValidator.ValidateAsync(request);
        if (!result.IsValid)
        {
            return BadRequest(result.ToDictionary());
        }
        
        var payment = await paymentsService.ProcessPaymentAsync(request.ToPaymentDetails(), ct);
        
        return Ok(ProcessPaymentResponse.FromPayment(payment));
    }

    [HttpGet("{id}")]
    public ActionResult<ProcessPaymentResponse?> GetPaymentAsync([FromRoute] string id)
    {
        var payment = paymentsService.GetPayment(id);
        if (payment == null)
        {
            return NotFound();
        }
        
        return Ok(payment);
    }
}