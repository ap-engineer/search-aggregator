using HtmlAgilityPack;
using SearchAggregator.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SearchAggregator.Api.Services.SearchEngines;

/// <summary>
/// Bing search engine implementation
/// </summary>
public class BingSearchEngine : ISearchEngine
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BingSearchEngine> _logger;

    public string Name => "Bing";

    public BingSearchEngine(HttpClient httpClient, ILogger<BingSearchEngine> logger)
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
            var url = $"https://www.bing.com/search?q={encodedTerm}&count=1";

            _logger.LogInformation("Searching Bing for term: {SearchTerm}", searchTerm);

            var response = await _httpClient.GetStringAsync(url, cancellationToken);
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            // Look for the results count element
            var countElement = doc.DocumentNode
                .SelectSingleNode("//span[@class='sb_count']") ??
                doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'sb_count')]");

            if (countElement != null)
            {
                var countText = countElement.InnerText;
                // Bing format: "1-10 of 1,234,567 results"
                var match = Regex.Match(countText, @"of\s+([\d,]+)\s+results", RegexOptions.IgnoreCase);
                
                if (match.Success)
                {
                    var numberStr = match.Groups[1].Value.Replace(",", "").Replace(".", "");
                    if (long.TryParse(numberStr, out var result))
                    {
                        _logger.LogInformation("Bing search for '{SearchTerm}' returned {Results} results", searchTerm, result);
                        return result;
                    }
                }
            }

            // Alternative: look for different count formats
            var altCountElement = doc.DocumentNode
                .SelectSingleNode("//div[contains(@class, 'b_rs')]//span[contains(text(), 'results')]");
            
            if (altCountElement != null)
            {
                var countText = altCountElement.InnerText;
                var match = Regex.Match(countText, @"([\d,]+)");
                
                if (match.Success)
                {
                    var numberStr = match.Value.Replace(",", "").Replace(".", "");
                    if (long.TryParse(numberStr, out var result))
                    {
                        _logger.LogInformation("Bing search for '{SearchTerm}' returned {Results} results (alternative parsing)", searchTerm, result);
                        return result;
                    }
                }
            }

            // Fallback: count result elements
            var resultNodes = doc.DocumentNode.SelectNodes("//li[@class='b_algo']") ??
                             doc.DocumentNode.SelectNodes("//div[contains(@class, 'b_algo')]");
            
            if (resultNodes != null)
            {
                var estimatedTotal = resultNodes.Count * 500000; // Rough estimate
                _logger.LogWarning("Could not parse exact Bing results for '{SearchTerm}', using estimate: {Results}", searchTerm, estimatedTotal);
                return estimatedTotal;
            }

            _logger.LogWarning("Could not parse Bing search results for term: {SearchTerm}", searchTerm);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching Bing for term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
