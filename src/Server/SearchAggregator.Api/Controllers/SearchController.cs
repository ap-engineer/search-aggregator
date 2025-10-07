using Microsoft.AspNetCore.Mvc;
using SearchAggregator.Api.DTOs;
using SearchAggregator.Api.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using SearchAggregator.Api.Services.Implementation;

namespace SearchAggregator.Api.Controllers;

/// <summary>
/// Controller for search hits
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class SearchController(SearchAggregatorService searchAggregatorService, ILogger<SearchController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery,
         MinLength(1, ErrorMessage = "Query must not be empty"),
         MaxLength(500, ErrorMessage = "Query must not exceed 500 characters")]
        string term, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest("Query parameter 'term' is required.");

        try
        {
            var response = await searchAggregatorService.AggregateAsync(term, ct);
            return Ok(response);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request cancelled for query: {Query}", term);
            return StatusCode(StatusCodes.Status499ClientClosedRequest, new { error = "Request cancelled." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during aggregation for query: {Query}", term);
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Internal server error." });
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