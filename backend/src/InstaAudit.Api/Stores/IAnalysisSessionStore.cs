using InstaAudit.Api.Models;

namespace InstaAudit.Api.Stores;

public interface IAnalysisSessionStore
{
    AnalysisSession Create(AnalysisResult result, DateTimeOffset expiresAt);
    AnalysisSession? Get(string token);
    bool MarkPaid(string token, string? checkoutSessionId);
    void SetCheckoutSessionId(string token, string checkoutSessionId);
}
