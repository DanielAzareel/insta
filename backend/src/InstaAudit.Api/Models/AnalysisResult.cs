namespace InstaAudit.Api.Models;

public class AnalysisResult
{
    public required IReadOnlyList<string> NotFollowingBack { get; init; }
    public required IReadOnlyList<string> Fans { get; init; }
    public required IReadOnlyList<string> Mutuals { get; init; }
    public required int FollowersCount { get; init; }
    public required int FollowingCount { get; init; }
}
