using System.Text.Json;
using Microsoft.Extensions.Options;
using SearchAggregator.Api.Config;
using SearchAggregator.Api.Services.Interfaces;

namespace SearchAggregator.Api.SearchEngines;

public sealed class WikipediaSearchEngine : ISearchEngine
{
    private readonly HttpClient _http;
    private readonly SearchEngineOptions.WikipediaOptions _cfg;

    public string Name => "Wikipedia";

    public WikipediaSearchEngine(HttpClient http, IOptions<SearchEngineOptions> opts)
    {
        _http = http;
        _cfg = opts.Value.Wikipedia;
        _http.BaseAddress = new Uri(_cfg.Endpoint);
        // Set user agent to avoid being blocked
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

    }

    public async Task<long> GetEstimatedHitsAsync(string term, CancellationToken ct)
    {
        var url = $"?action=query&list=search&format=json&srsearch={Uri.EscapeDataString(term)}";
        var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);

        if (json.TryGetProperty("query", out var query) &&
            query.TryGetProperty("searchinfo", out var info) &&
            info.TryGetProperty("totalhits", out var total) &&
            total.ValueKind == JsonValueKind.Number)
        {
            return total.GetInt64();
        }

        return 0L;
    }
}