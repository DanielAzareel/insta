using InstaAudit.Api.DTOs;
using InstaAudit.Api.Models;
using InstaAudit.Api.Options;
using Microsoft.Extensions.Options;
using Stripe.Checkout;

namespace InstaAudit.Api.Services;

public class StripeCheckoutService : IStripeCheckoutService
{
    private readonly AppOptions _appOptions;

    public StripeCheckoutService(IOptions<AppOptions> appOptions)
    {
        _appOptions = appOptions.Value;
    }

    public async Task<CheckoutResponse> CreateCheckoutSessionAsync(AnalysisSession session, CancellationToken cancellationToken)
    {
        var service = new SessionService();

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{_appOptions.FrontendUrl}/success?analysisToken={session.Token}",
            CancelUrl = $"{_appOptions.FrontendUrl}/cancel?analysisToken={session.Token}",
            Metadata = new Dictionary<string, string>
            {
                ["analysisToken"] = session.Token
            },
            LineItems =
            [
                new SessionLineItemOptions
                {
                    Quantity = 1,
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "mxn",
                        UnitAmount = _appOptions.PriceMxn * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "InstaAudit - Reporte completo"
                        }
                    }
                }
            ]
        };

        var stripeSession = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return new CheckoutResponse { CheckoutUrl = stripeSession.Url! };
    }
}
