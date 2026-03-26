using InstaAudit.Api.Options;
using InstaAudit.Api.Services;
using InstaAudit.Api.Stores;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StripeOptions>(builder.Configuration.GetSection("Stripe"));
builder.Services.Configure<AppOptions>(builder.Configuration.GetSection("App"));

builder.Services.AddSingleton<IAnalysisSessionStore, InMemoryAnalysisSessionStore>();
builder.Services.AddScoped<IInstagramAnalyzerService, InstagramAnalyzerService>();
builder.Services.AddScoped<IExcelExportService, ExcelExportService>();
builder.Services.AddScoped<IStripeCheckoutService, StripeCheckoutService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", p => p.WithOrigins(corsOrigins).AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

var stripeSecret = builder.Configuration["Stripe:SecretKey"];
if (!string.IsNullOrWhiteSpace(stripeSecret))
{
    StripeConfiguration.ApiKey = stripeSecret;
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("FrontendPolicy");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
