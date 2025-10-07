using System.Diagnostics;
using SearchAggregator.Api.Models;
using SearchAggregator.Api.Services.Interfaces;

namespace SearchAggregator.Api.Services.Implementation;

/// <summary>
/// Service that aggregates search results from multiple search engines
/// </summary>
public sealed class SearchAggregatorService(IEnumerable<ISearchEngine> engines)
{
    public async Task<IReadOnlyList<Result>> AggregateAsync(
        string term, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(term))
            return [];

        var tasks = engines.Select(async e =>
        {
            try
            {
                var hits = await e.GetEstimatedHitsAsync(term, ct);
                return new Result(e.Name, hits);
            }
            catch
            {
                return new Result(e.Name, 0);
            }
        });

        return await Task.WhenAll(tasks);
    }

    public sealed record Result(string Engine, long Hits);
}