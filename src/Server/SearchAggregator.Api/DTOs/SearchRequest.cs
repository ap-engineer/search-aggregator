using System.ComponentModel.DataAnnotations;

namespace SearchAggregator.Api.DTOs;

/// <summary>
/// Request DTO for search operations
/// </summary>
public class SearchRequest
{
    [Required(ErrorMessage = "Query is required")]
    [MinLength(1, ErrorMessage = "Query must not be empty")]
    [MaxLength(500, ErrorMessage = "Query must not exceed 500 characters")]
    public string Query { get; set; } = string.Empty;
}
