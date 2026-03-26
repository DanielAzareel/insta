using InstaAudit.Api.Models;

namespace InstaAudit.Api.Services;

public interface IInstagramAnalyzerService
{
    Task<AnalysisResult> AnalyzeAsync(Stream followersJsonStream, Stream followingJsonStream, CancellationToken cancellationToken);
}
