namespace SearchAggregator.Api.DTOs;

/// <summary>
/// Response DTO for search operations
/// </summary>
public class SearchResponse
{
    public string Query { get; set; } = string.Empty;
    public List<string> SearchTerms { get; set; } = new();
    public List<SearchEngineResult> SearchEngines { get; set; } = new();
    public DateTime SearchedAt { get; set; }
    public double TotalSearchTimeMs { get; set; }
    public bool HasErrors { get; set; }
}

/// <summary>
/// Represents search results from a specific search engine
/// </summary>
public class SearchEngineResult
{
    public string Name { get; set; } = string.Empty;
    public long TotalHits { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}
