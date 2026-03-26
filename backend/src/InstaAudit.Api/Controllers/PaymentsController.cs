using InstaAudit.Api.DTOs;
using InstaAudit.Api.Services;
using InstaAudit.Api.Stores;
using Microsoft.AspNetCore.Mvc;

namespace InstaAudit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IAnalysisSessionStore _sessionStore;
    private readonly IStripeCheckoutService _stripeCheckoutService;

    public PaymentsController(IAnalysisSessionStore sessionStore, IStripeCheckoutService stripeCheckoutService)
    {
        _sessionStore = sessionStore;
        _stripeCheckoutService = stripeCheckoutService;
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<CheckoutResponse>> Checkout([FromBody] CheckoutRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.AnalysisToken))
        {
            return BadRequest("analysisToken es requerido.");
        }

        var session = _sessionStore.Get(request.AnalysisToken);
        if (session is null) return NotFound("Análisis no encontrado o expirado.");

        var checkout = await _stripeCheckoutService.CreateCheckoutSessionAsync(session, cancellationToken);
        return Ok(checkout);
    }

    [HttpGet("status/{analysisToken}")]
    public ActionResult<PaymentStatusResponse> Status(string analysisToken)
    {
        var session = _sessionStore.Get(analysisToken);
        if (session is null)
        {
            return NotFound(new PaymentStatusResponse { Paid = false });
        }

        return Ok(new PaymentStatusResponse
        {
            Paid = session.Paid,
            ExpiresAt = session.ExpiresAt
        });
    }
}
