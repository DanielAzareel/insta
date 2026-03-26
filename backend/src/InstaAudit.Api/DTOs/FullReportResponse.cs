namespace InstaAudit.Api.DTOs;

public class FullReportResponse
{
    public required string AnalysisToken { get; init; }
    public required IReadOnlyList<string> NotFollowingBack { get; init; }
    public required IReadOnlyList<string> Fans { get; init; }
    public required IReadOnlyList<string> Mutuals { get; init; }
}
