namespace InstaAudit.Api.Models;

public class AnalysisSession
{
    public required string Token { get; init; }
    public required DateTimeOffset ExpiresAt { get; set; }
    public required AnalysisResult Result { get; init; }
    public bool Paid { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
}
