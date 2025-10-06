using SearchAggregator.Api.Services;
using SearchAggregator.Api.Services.Interfaces;
using SearchAggregator.Api.Services.SearchEngines;
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
    c.SwaggerDoc("v1", new() { 
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

// Configure HTTP clients for search engines
builder.Services.AddHttpClient<GoogleSearchEngine>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<BingSearchEngine>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<DuckDuckGoSearchEngine>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register search engines
builder.Services.AddScoped<ISearchEngine, GoogleSearchEngine>();
builder.Services.AddScoped<ISearchEngine, BingSearchEngine>();
builder.Services.AddScoped<ISearchEngine, DuckDuckGoSearchEngine>();

// Register search aggregator service
builder.Services.AddScoped<ISearchAggregatorService, SearchAggregatorService>();

// Configure CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Search Aggregator API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
    app.UseCors("AllowAll");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Add a simple health check endpoint
app.MapGet("/health", () => new { Status = "Healthy", Timestamp = DateTime.UtcNow });

app.Run();