using InstaAudit.Api.Options;
using InstaAudit.Api.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace InstaAudit.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IAnalysisSessionStore _sessionStore;
    private readonly StripeOptions _stripeOptions;

    public WebhooksController(IAnalysisSessionStore sessionStore, IOptions<StripeOptions> stripeOptions)
    {
        _sessionStore = sessionStore;
        _stripeOptions = stripeOptions.Value;
    }

    [HttpPost("stripe")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signatureHeader = Request.Headers["Stripe-Signature"];

        Event stripeEvent;
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signatureHeader, _stripeOptions.WebhookSecret);
        }
        catch (Exception)
        {
            return BadRequest();
        }

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;
            var analysisToken = session?.Metadata?.GetValueOrDefault("analysisToken");

            if (!string.IsNullOrWhiteSpace(analysisToken))
            {
                _sessionStore.MarkPaid(analysisToken, session?.Id);
            }
        }

        return Ok();
    }
}
