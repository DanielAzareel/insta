namespace InstaAudit.Api.DTOs;

public class AnalysisPreviewResponse
{
    public required string AnalysisToken { get; init; }
    public required int FollowersCount { get; init; }
    public required int FollowingCount { get; init; }
    public required int MutualCount { get; init; }
    public required int NotFollowingBackCount { get; init; }
    public required int FansCount { get; init; }
    public required IReadOnlyList<string> PreviewNotFollowingBack { get; init; }
    public required IReadOnlyList<string> PreviewFans { get; init; }
    public required IReadOnlyList<string> PreviewMutuals { get; init; }
    public required int LockedNotFollowingBackCount { get; init; }
    public required int LockedFansCount { get; init; }
    public required int LockedMutualsCount { get; init; }
    public required int PriceMxn { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}
