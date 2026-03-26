using InstaAudit.Api.Models;

namespace InstaAudit.Api.Services;

public interface IExcelExportService
{
    byte[] GenerateReport(AnalysisResult result);
}
