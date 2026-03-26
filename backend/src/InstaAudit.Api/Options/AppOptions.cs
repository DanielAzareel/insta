namespace InstaAudit.Api.Options;

public class AppOptions
{
    public string FrontendUrl { get; set; } = "http://localhost:5173";
    public int PriceMxn { get; set; } = 25;
    public int SessionDurationMinutes { get; set; } = 30;
}
