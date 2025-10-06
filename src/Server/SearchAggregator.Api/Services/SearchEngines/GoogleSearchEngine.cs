using HtmlAgilityPack;
using SearchAggregator.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SearchAggregator.Api.Services.SearchEngines;

/// <summary>
/// Google search engine implementation
/// </summary>
public class GoogleSearchEngine : ISearchEngine
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleSearchEngine> _logger;

    public string Name => "Google";

    public GoogleSearchEngine(HttpClient httpClient, ILogger<GoogleSearchEngine> logger)
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
            var url = $"https://www.google.com/search?q={encodedTerm}&num=1";

            _logger.LogInformation("Searching Google for term: {SearchTerm}", searchTerm);

            var response = await _httpClient.GetStringAsync(url, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            // Look for the results stats element
            var statsElement = doc.DocumentNode
                .SelectSingleNode("//div[@id='result-stats']") ??
                doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'result-stats')]");

            if (statsElement != null)
            {
                var statsText = statsElement.InnerText;
                var match = Regex.Match(statsText, @"[\d,]+");
                
                if (match.Success)
                {
                    var numberStr = match.Value.Replace(",", "").Replace(".", "");
                    if (long.TryParse(numberStr, out var result))
                    {
                        _logger.LogInformation("Google search for '{SearchTerm}' returned {Results} results", searchTerm, result);
                        return result;
                    }
                }
            }

            // Fallback: try to count search result divs
            var resultNodes = doc.DocumentNode.SelectNodes("//div[@class='g']") ??
                             doc.DocumentNode.SelectNodes("//div[contains(@class, 'g')]");
            
            if (resultNodes != null)
            {
                // Estimate based on typical Google results per page
                var estimatedTotal = resultNodes.Count * 1000000; // Very rough estimate
                _logger.LogWarning("Could not parse exact Google results for '{SearchTerm}', using estimate: {Results}", searchTerm, estimatedTotal);
                return estimatedTotal;
            }

            _logger.LogWarning("Could not parse Google search results for term: {SearchTerm}", searchTerm);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Google for term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
