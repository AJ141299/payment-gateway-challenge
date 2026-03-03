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
    [ProducesResponseType(typeof(ProcessPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ProcessPaymentAsync(ProcessPaymentRequest request, CancellationToken ct)
    {
        var result = await processPaymentValidator.ValidateAsync(request, ct);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => new 
            {
                code = e.ErrorCode,
                message = e.ErrorMessage
            });
            return BadRequest(new { errors });
        }
        
        var payment = await paymentsService.ProcessPaymentAsync(request.ToPaymentDetails(), ct);
        
        return Ok(ProcessPaymentResponse.FromPayment(payment));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPaymentAsync([FromRoute] string id)
    {
        var payment = paymentsService.GetPayment(id);
        if (payment == null)
        {
            return NotFound();
        }
        
        return Ok(GetPaymentResponse.FromPayment(payment));
    }
}