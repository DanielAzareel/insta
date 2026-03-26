namespace InstaAudit.Api.DTOs;

public class PaymentStatusResponse
{
    public required bool Paid { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
}
