using SearchAggregator.Api.DTOs;

namespace SearchAggregator.Api.Services.Interfaces;

/// <summary>
/// Interface for the search aggregator service
/// </summary>
public interface ISearchAggregatorService
{
    /// <summary>
    /// Performs search across all configured search engines
    /// </summary>
    /// <param name="query">The search query (can contain multiple words)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Aggregated search results</returns>
    Task<SearchResponse> AggregateAsync(string query, CancellationToken ct = default);
}