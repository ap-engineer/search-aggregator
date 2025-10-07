using System.Text.Json;
using Microsoft.Extensions.Options;
using SearchAggregator.Api.Options;
using SearchAggregator.Api.Services.Interfaces;

namespace SearchAggregator.Api.SearchEngines;

/// <summary>
/// Google search engine implementation
/// </summary>
public sealed class GoogleSearchEngine : ISearchEngine
{
    private readonly HttpClient _http;
    private readonly GoogleOptions _cfg;

    public string Name => "Google";

    public GoogleSearchEngine(HttpClient http, IOptions<SearchEngineOptions> opts)
    {
        _http = http;
        _cfg = opts.Value.Google;
        _http.BaseAddress = new Uri("https://www.googleapis.com/customsearch/v1");
    }

    public async Task<long> GetEstimatedHitsAsync(string term, CancellationToken ct)
    {
        var url = $"?key={_cfg.ApiKey}&cx={_cfg.SearchEngineId}&q={Uri.EscapeDataString(term)}";
        var json = await _http.GetFromJsonAsync<JsonElement>(url, ct);

        if (json.TryGetProperty("searchInformation", out var info) &&
            info.TryGetProperty("totalResults", out var total))
        {
            if (total.ValueKind == JsonValueKind.String &&
                long.TryParse(total.GetString(), out var parsed))
            {
                return parsed;
            }
        }

        return 0L;
    }
}