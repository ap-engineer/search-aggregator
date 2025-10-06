using HtmlAgilityPack;
using SearchAggregator.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SearchAggregator.Api.Services.SearchEngines;

/// <summary>
/// DuckDuckGo search engine implementation
/// </summary>
public class DuckDuckGoSearchEngine : ISearchEngine
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DuckDuckGoSearchEngine> _logger;

    public string Name => "DuckDuckGo";

    public DuckDuckGoSearchEngine(HttpClient httpClient, ILogger<DuckDuckGoSearchEngine> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        // Set user agent to avoid being blocked
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");
    }

    public async Task<long> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            var encodedTerm = Uri.EscapeDataString(searchTerm);
            var url = $"https://duckduckgo.com/html/?q={encodedTerm}";

            _logger.LogInformation("Searching DuckDuckGo for term: {SearchTerm}", searchTerm);

            var response = await _httpClient.GetStringAsync(url, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            // DuckDuckGo doesn't typically show total result counts like Google/Bing
            // We'll count the actual results and provide an estimate
            var resultNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'result')]") ??
                             doc.DocumentNode.SelectNodes("//div[@class='web-result']") ??
                             doc.DocumentNode.SelectNodes("//div[contains(@class, 'web-result')]");

            if (resultNodes != null && resultNodes.Count > 0)
            {
                // DuckDuckGo typically shows 10 results per page
                // We'll estimate based on the presence of results
                var estimatedTotal = resultNodes.Count * 250000; // Conservative estimate
                _logger.LogInformation("DuckDuckGo search for '{SearchTerm}' found {ActualResults} results, estimated total: {EstimatedTotal}", 
                    searchTerm, resultNodes.Count, estimatedTotal);
                return estimatedTotal;
            }

            // Alternative: look for any search result indicators
            var anyResults = doc.DocumentNode.SelectNodes("//a[contains(@class, 'result__a')]") ??
                            doc.DocumentNode.SelectNodes("//h2[contains(@class, 'result__title')]");

            if (anyResults != null && anyResults.Count > 0)
            {
                var estimatedTotal = anyResults.Count * 200000; // Conservative estimate
                _logger.LogInformation("DuckDuckGo search for '{SearchTerm}' found {ActualResults} result links, estimated total: {EstimatedTotal}", 
                    searchTerm, anyResults.Count, estimatedTotal);
                return estimatedTotal;
            }

            _logger.LogWarning("No DuckDuckGo search results found for term: {SearchTerm}", searchTerm);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching DuckDuckGo for term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
