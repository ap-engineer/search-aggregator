using SearchAggregator.Api.Config;
using SearchAggregator.Api.SearchEngines;
using SearchAggregator.Api.Services.Implementation;
using SearchAggregator.Api.Services.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Search Aggregator API",
        Version = "v1",
        Description = "API for aggregating search results from multiple search engines"
    });

    // Include XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Configure HTTP clients for search engine
builder.Services.AddHttpClient<WikipediaSearchEngine>(client => { client.Timeout = TimeSpan.FromSeconds(30); });
builder.Services.AddHttpClient<GoogleSearchEngine>(client => { client.Timeout = TimeSpan.FromSeconds(30); });


builder.Services.Configure<SearchEngineOptions>(
    builder.Configuration.GetSection("SearchEngines"));

builder.Services.AddScoped<ISearchEngine, GoogleSearchEngine>();
builder.Services.AddScoped<ISearchEngine, WikipediaSearchEngine>();
builder.Services.AddScoped<SearchAggregatorService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();