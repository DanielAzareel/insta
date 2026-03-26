using ClosedXML.Excel;
using InstaAudit.Api.Models;

namespace InstaAudit.Api.Services;

public class ExcelExportService : IExcelExportService
{
    public byte[] GenerateReport(AnalysisResult result)
    {
        using var workbook = new XLWorkbook();
        AddSheet(workbook, "No me siguen", result.NotFollowingBack);
        AddSheet(workbook, "Fans", result.Fans);
        AddSheet(workbook, "Mutuos", result.Mutuals);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static void AddSheet(XLWorkbook workbook, string sheetName, IReadOnlyList<string> users)
    {
        var ws = workbook.Worksheets.Add(sheetName);
        ws.Cell(1, 1).Value = "Username";
        ws.Cell(1, 1).Style.Font.Bold = true;

        for (var i = 0; i < users.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = users[i];
        }

        ws.Columns().AdjustToContents();
    }
}
