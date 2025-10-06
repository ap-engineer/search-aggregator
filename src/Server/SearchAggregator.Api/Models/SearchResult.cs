namespace SearchAggregator.Api.Models;

/// <summary>
/// Represents the search result from a single search engine
/// </summary>
public class SearchResult
{
    public string SearchEngine { get; set; } = string.Empty;
    public long TotalHits { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Represents the aggregated search results from all search engines
/// </summary>
public class AggregatedSearchResult
{
    public string Query { get; set; } = string.Empty;
    public List<string> SearchTerms { get; set; } = new();
    public List<SearchResult> Results { get; set; } = new();
    public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
    public TimeSpan TotalSearchTime { get; set; }
}
