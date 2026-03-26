using InstaAudit.Api.DTOs;
using InstaAudit.Api.Models;
using InstaAudit.Api.Options;
using InstaAudit.Api.Services;
using InstaAudit.Api.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InstaAudit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnalysisController : ControllerBase
{
    private const int PreviewLimit = 10;
    private readonly IInstagramAnalyzerService _analyzerService;
    private readonly IAnalysisSessionStore _sessionStore;
    private readonly IExcelExportService _excelExportService;
    private readonly AppOptions _appOptions;

    public AnalysisController(
        IInstagramAnalyzerService analyzerService,
        IAnalysisSessionStore sessionStore,
        IExcelExportService excelExportService,
        IOptions<AppOptions> appOptions)
    {
        _analyzerService = analyzerService;
        _sessionStore = sessionStore;
        _excelExportService = excelExportService;
        _appOptions = appOptions.Value;
    }

    [HttpPost("preview")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<AnalysisPreviewResponse>> Preview(
        [FromForm] IFormFile? followersFile,
        [FromForm] IFormFile? followingFile,
        CancellationToken cancellationToken)
    {
        if (followersFile is null || followingFile is null)
        {
            return BadRequest("Debes enviar followersFile y followingFile.");
        }

        if (!followersFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ||
            !followingFile.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Ambos archivos deben ser JSON.");
        }

        try
        {
            await using var followersStream = followersFile.OpenReadStream();
            await using var followingStream = followingFile.OpenReadStream();

            var result = await _analyzerService.AnalyzeAsync(followersStream, followingStream, cancellationToken);
            var expiresAt = DateTimeOffset.UtcNow.AddMinutes(_appOptions.SessionDurationMinutes);
            var session = _sessionStore.Create(result, expiresAt);

            return Ok(ToPreviewResponse(session, result));
        }
        catch (Exception)
        {
            return BadRequest("No se pudo parsear la estructura JSON de Instagram.");
        }
    }

    [HttpGet("full/{analysisToken}")]
    public ActionResult<FullReportResponse> Full(string analysisToken)
    {
        var session = _sessionStore.Get(analysisToken);
        if (session is null) return NotFound("Análisis no encontrado o expirado.");
        if (!session.Paid) return StatusCode(StatusCodes.Status402PaymentRequired, "Pago requerido.");

        return Ok(new FullReportResponse
        {
            AnalysisToken = analysisToken,
            NotFollowingBack = session.Result.NotFollowingBack,
            Fans = session.Result.Fans,
            Mutuals = session.Result.Mutuals
        });
    }

    [HttpGet("export/{analysisToken}")]
    public IActionResult Export(string analysisToken)
    {
        var session = _sessionStore.Get(analysisToken);
        if (session is null) return NotFound("Análisis no encontrado o expirado.");
        if (!session.Paid) return StatusCode(StatusCodes.Status402PaymentRequired, "Pago requerido.");

        var bytes = _excelExportService.GenerateReport(session.Result);
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"instaaudit-{analysisToken}.xlsx");
    }

    private AnalysisPreviewResponse ToPreviewResponse(AnalysisSession session, AnalysisResult result)
    {
        var previewNotFollowing = result.NotFollowingBack.Take(PreviewLimit).ToList();
        var previewFans = result.Fans.Take(PreviewLimit).ToList();
        var previewMutuals = result.Mutuals.Take(PreviewLimit).ToList();

        return new AnalysisPreviewResponse
        {
            AnalysisToken = session.Token,
            FollowersCount = result.FollowersCount,
            FollowingCount = result.FollowingCount,
            MutualCount = result.Mutuals.Count,
            NotFollowingBackCount = result.NotFollowingBack.Count,
            FansCount = result.Fans.Count,
            PreviewNotFollowingBack = previewNotFollowing,
            PreviewFans = previewFans,
            PreviewMutuals = previewMutuals,
            LockedNotFollowingBackCount = Math.Max(0, result.NotFollowingBack.Count - PreviewLimit),
            LockedFansCount = Math.Max(0, result.Fans.Count - PreviewLimit),
            LockedMutualsCount = Math.Max(0, result.Mutuals.Count - PreviewLimit),
            PriceMxn = _appOptions.PriceMxn,
            ExpiresAt = session.ExpiresAt
        };
    }
}
