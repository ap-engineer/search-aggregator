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
public class SearchController(SearchAggregatorService searchAggregatorService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string term, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest("Query parameter 'term' is required.");

        var results = await searchAggregatorService.AggregateAsync(term, ct);
        return Ok(results);
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