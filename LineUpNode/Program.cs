using LineUpNode.Services;
using LineUpNode.Services.Scrapers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IScraperService, KinotekaScraper>();
builder.Services.AddScoped<IScraperService, IluzjonScraper>();
builder.Services.AddScoped<IScraperService, AmondoScraper>();
// kolejne scrappery

builder.Services.AddScoped<ScraperService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();