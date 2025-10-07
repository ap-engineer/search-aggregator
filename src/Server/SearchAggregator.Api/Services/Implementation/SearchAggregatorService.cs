using System.Diagnostics;
using SearchAggregator.Api.DTOs;
using SearchAggregator.Api.Services.Interfaces;

namespace SearchAggregator.Api.Services.Implementation;

/// <summary>
/// Service that aggregates search results from multiple search engines
/// </summary>
public sealed class SearchAggregatorService(IEnumerable<ISearchEngine> engines, ILogger<SearchAggregatorService> logger)
    : ISearchAggregatorService
{
    public async Task<SearchResponse> AggregateAsync(string query, CancellationToken ct)
    {
        var response = new SearchResponse(
            query: query?.Trim() ?? string.Empty,
            searchedAt: DateTime.UtcNow
        );

        if (string.IsNullOrWhiteSpace(query))
            return response;

        var stopwatch = Stopwatch.StartNew();

        var tasks = engines.Select(async e =>
        {
            try
            {
                var hits = await e.GetEstimatedHitsAsync(query, ct);
                return new SearchEngineResult(e.Name, hits, true, null);
            }
            catch (Exception ex)
            {
                return new SearchEngineResult(e.Name, 0, false, ex.Message);
            }
        });

        var engineResults = await Task.WhenAll(tasks);
        stopwatch.Stop();

        response.PopulateValues(
            totalHits: engineResults.Select(eR => eR.TotalHits).Sum(),
            searchEngines: engineResults,
            totalSearchTimeMs: stopwatch.Elapsed.TotalMilliseconds,
            hasErrors: engineResults.Any(e => !e.IsSuccess)
        );

        return response;
    }
}