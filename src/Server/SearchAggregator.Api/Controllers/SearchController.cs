using Microsoft.AspNetCore.Mvc;
using SearchAggregator.Api.DTOs;
using SearchAggregator.Api.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace SearchAggregator.Api.Controllers;

/// <summary>
/// Controller for search operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly ISearchAggregatorService _searchAggregatorService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(
        ISearchAggregatorService searchAggregatorService,
        ILogger<SearchController> logger)
    {
        _searchAggregatorService = searchAggregatorService;
        _logger = logger;
    }

    /// <summary>
    /// Searches across multiple search engines and returns aggregated hit counts
    /// </summary>
    /// <param name="query">The search query (can contain multiple words)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Aggregated search results showing hit counts from each search engine</returns>
    /// <response code="200">Returns the aggregated search results</response>
    /// <response code="400">If the query is invalid</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SearchResponse>> Search(
        [FromQuery, Required, MinLength(1), MaxLength(500)] string query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { nameof(query), new[] { "Query cannot be empty" } }
                }));
            }

            _logger.LogInformation("Received search request for query: {Query}", query);

            var result = await _searchAggregatorService.SearchAsync(query, cancellationToken);

            var response = new SearchResponse
            {
                Query = result.Query,
                SearchTerms = result.SearchTerms,
                SearchedAt = result.SearchedAt,
                TotalSearchTimeMs = result.TotalSearchTime.TotalMilliseconds,
                SearchEngines = result.Results.Select(r => new SearchEngineResult
                {
                    Name = r.SearchEngine,
                    TotalHits = r.TotalHits,
                    IsSuccess = r.IsSuccess,
                    ErrorMessage = r.ErrorMessage
                }).ToList(),
                HasErrors = result.Results.Any(r => !r.IsSuccess)
            };

            _logger.LogInformation("Search completed for query: {Query}, returned {EngineCount} results", 
                query, response.SearchEngines.Count);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing search request for query: {Query}", query);
            
            return Problem(
                title: "Search Error",
                detail: "An error occurred while processing your search request.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    /// <summary>
    /// Health check endpoint for the search service
    /// </summary>
    /// <returns>Service health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public ActionResult GetHealth()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
}
