using SearchAggregator.Api.Models;
using SearchAggregator.Api.Services.Interfaces;
using System.Diagnostics;

namespace SearchAggregator.Api.Services;

/// <summary>
/// Service that aggregates search results from multiple search engines
/// </summary>
public class SearchAggregatorService(
    IEnumerable<ISearchEngine> searchEngines,
    ILogger<SearchAggregatorService> logger)
    : ISearchAggregatorService
{
    public async Task<AggregatedSearchResult> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        
        logger.LogInformation("Starting search aggregation for query: {Query}", query);

        var result = new AggregatedSearchResult
        {
            Query = query,
            SearchTerms = ExtractSearchTerms(query),
            SearchedAt = DateTime.UtcNow
        };

        // Create tasks for each search engine
        var searchTasks = searchEngines.Select(async engine =>
        {
            try
            {
                logger.LogInformation("Starting search with {SearchEngine} for query: {Query}", engine.Name, query);
                
                long totalHits = 0;
                
                // Search for each term and sum the results
                foreach (var term in result.SearchTerms)
                {
                    var hits = await engine.SearchAsync(term, cancellationToken);
                    totalHits += hits;
                    
                    logger.LogInformation("{SearchEngine} found {Hits} hits for term: {Term}", 
                        engine.Name, hits, term);
                }

                return new SearchResult
                {
                    SearchEngine = engine.Name,
                    TotalHits = totalHits,
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching with {SearchEngine} for query: {Query}", engine.Name, query);
                
                return new SearchResult
                {
                    SearchEngine = engine.Name,
                    TotalHits = 0,
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        });

        // Wait for all searches to complete
        var searchResults = await Task.WhenAll(searchTasks);
        result.Results.AddRange(searchResults);

        stopwatch.Stop();
        result.TotalSearchTime = stopwatch.Elapsed;

        logger.LogInformation("Search aggregation completed for query: {Query} in {ElapsedMs}ms", 
            query, stopwatch.ElapsedMilliseconds);

        return result;
    }

    private static List<string> ExtractSearchTerms(string query)
    {
        // Split the query into individual words/terms
        return query
            .Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(term => term.Trim())
            .Where(term => !string.IsNullOrEmpty(term))
            .ToList();
    }
}
