using LineUpNode.Services;
using LineUpNode.Services.Scrapers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IScraperService, AmondoScraper>();
builder.Services.AddScoped<IScraperService, AtlanticScraper>();
builder.Services.AddScoped<IScraperService, IluzjonScraper>();
builder.Services.AddScoped<IScraperService, KinotekaScraper>();
builder.Services.AddScoped<IScraperService, KulturaScraper>();
builder.Services.AddScoped<IScraperService, LunaScraper>();
builder.Services.AddScoped<IScraperService, MuranowScraper>();
builder.Services.AddScoped<IScraperService, PrahaScraper>();
builder.Services.AddScoped<IScraperService, WislaScraper>();
// kolejne scrappery alfabetycznie

builder.Services.AddScoped<ScraperService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();