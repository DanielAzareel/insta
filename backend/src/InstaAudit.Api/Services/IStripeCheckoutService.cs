using InstaAudit.Api.DTOs;
using InstaAudit.Api.Models;

namespace InstaAudit.Api.Services;

public interface IStripeCheckoutService
{
    Task<CheckoutResponse> CreateCheckoutSessionAsync(AnalysisSession session, CancellationToken cancellationToken);
}
