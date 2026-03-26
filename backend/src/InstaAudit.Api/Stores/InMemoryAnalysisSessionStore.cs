using System.Collections.Concurrent;
using InstaAudit.Api.Models;

namespace InstaAudit.Api.Stores;

public class InMemoryAnalysisSessionStore : IAnalysisSessionStore
{
    private readonly ConcurrentDictionary<string, AnalysisSession> _sessions = new();

    public AnalysisSession Create(AnalysisResult result, DateTimeOffset expiresAt)
    {
        CleanupExpired();
        var token = Guid.NewGuid().ToString("N");
        var session = new AnalysisSession
        {
            Token = token,
            ExpiresAt = expiresAt,
            Result = result,
            Paid = false
        };

        _sessions[token] = session;
        return session;
    }

    public AnalysisSession? Get(string token)
    {
        CleanupExpired();
        if (_sessions.TryGetValue(token, out var session) && session.ExpiresAt > DateTimeOffset.UtcNow)
        {
            return session;
        }

        _sessions.TryRemove(token, out _);
        return null;
    }

    public bool MarkPaid(string token, string? checkoutSessionId)
    {
        var session = Get(token);
        if (session is null) return false;

        session.Paid = true;
        session.StripeCheckoutSessionId = checkoutSessionId;
        return true;
    }

    public void SetCheckoutSessionId(string token, string checkoutSessionId)
    {
        var session = Get(token);
        if (session is null) return;
        session.StripeCheckoutSessionId = checkoutSessionId;
    }

    private void CleanupExpired()
    {
        var now = DateTimeOffset.UtcNow;
        foreach (var item in _sessions)
        {
            if (item.Value.ExpiresAt <= now)
            {
                _sessions.TryRemove(item.Key, out _);
            }
        }
    }
}
