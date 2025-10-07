namespace SearchAggregator.Api.Options;

public sealed class GoogleOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public string SearchEngineId { get; init; } = string.Empty;
}