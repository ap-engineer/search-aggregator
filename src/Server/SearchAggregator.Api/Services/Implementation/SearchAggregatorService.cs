using System.Collections.Concurrent;
using System.Diagnostics;
using SearchAggregator.Api.DTOs;
using SearchAggregator.Api.Services.Interfaces;

namespace SearchAggregator.Api.Services.Implementation;

/// <summary>
/// Aggregates results across multiple search engines.
/// Supports multi-word queries and runs engine queries concurrently.
/// Thread-safe and cancellation-aware.
/// </summary>
public sealed class SearchAggregatorService(
    IEnumerable<ISearchEngine> engines,
    ILogger<SearchAggregatorService> logger) : ISearchAggregatorService
{
    public async Task<SearchResponse> AggregateAsync(string query, CancellationToken ct)
    {
        var cleanedQuery = query.Trim();
        var response = new SearchResponse(query: cleanedQuery, searchedAt: DateTime.UtcNow);

        if (string.IsNullOrWhiteSpace(cleanedQuery))
            return response;

        // Split query into !distinct! word
        var terms = cleanedQuery
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var stopwatch = Stopwatch.StartNew();
        // Bag is just more efficient whe order is not important my Microsoft docs
        var resultsBag = new ConcurrentBag<SearchEngineResult>();

        // One task per engine
        var engineTasks = engines.Select(async engine =>
        {
            var totalHits = 0L;
            var success = true;
            string? errorMessage = null;

            try
            {
                // Run all terms concurrently for this engine
                var termTasks = terms.Select(async term =>
                {
                    try
                    {
                        ct.ThrowIfCancellationRequested();
                        return await engine.GetEstimatedHitsAsync(term, ct);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex,
                            "Error querying {Engine} for term '{Term}'", engine.Name, term);
                        return 0L;
                    }
                });

                var termResults = await Task.WhenAll(termTasks);
                totalHits = termResults.Sum();
            }
            catch (OperationCanceledException)
            {
                success = false;
                errorMessage = "Cancelled";
                logger.LogInformation("Search cancelled for {Engine}", engine.Name);
            }
            catch (Exception ex)
            {
                success = false;
                errorMessage = ex.Message;
                logger.LogError(ex, "Error querying {Engine}", engine.Name);
            }

            resultsBag.Add(new SearchEngineResult(engine.Name, totalHits, success, errorMessage));
        });

        await Task.WhenAll(engineTasks);
        stopwatch.Stop();

        var results = resultsBag.ToArray();
        response.PopulateValues(
            totalHits: results.Sum(r => r.TotalHits),
            searchEngines: results,
            totalSearchTimeMs: stopwatch.Elapsed.TotalMilliseconds,
            hasErrors: results.Any(r => !r.IsSuccess)
        );

        return response;
    }
}