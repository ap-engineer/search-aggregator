namespace SearchAggregator.Api.DTOs;

/// <summary>
/// Response DTO for search operations
/// </summary>
public class SearchResponse(string query, DateTime searchedAt)
{
    public string Query { get; init; } = query;
    public long TotalHits { get; private set; }
    public IEnumerable<SearchEngineResult> SearchEngines { get; set; } = [];
    public DateTime SearchedAt { get; private init; } = searchedAt;
    public double TotalSearchTimeMs { get; private set; }
    public bool HasErrors { get; private set; }

    public void PopulateValues(
        long totalHits,
        IEnumerable<SearchEngineResult> searchEngines,
        double totalSearchTimeMs,
        bool hasErrors)
    {
        TotalHits = totalHits;
        SearchEngines = searchEngines;
        TotalSearchTimeMs = totalSearchTimeMs;
        HasErrors = hasErrors;
    }
}