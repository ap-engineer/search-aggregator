namespace SearchAggregator.Api.DTOs;

/// <summary>
/// Represents search results from a specific search engine
/// </summary>
public class SearchEngineResult(string name, long hits, bool isSuccess, string? errorMessage)
{
    public string Name { get; private set; } = name;
    public long TotalHits { get; private set; } = hits;
    public bool IsSuccess { get; private set; } = isSuccess;
    public string? ErrorMessage { get; private set; } = errorMessage;
}