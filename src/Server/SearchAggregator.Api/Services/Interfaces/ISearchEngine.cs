namespace SearchAggregator.Api.Services.Interfaces;

/// <summary>
/// Interface for search engine implementations
/// </summary>
public interface ISearchEngine
{
    /// <summary>
    /// Name of the search engine
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Searches for a single term and returns the number of hits
    /// </summary>
    /// <param name="searchTerm">The term to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of search hits</returns>
    Task<long> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
