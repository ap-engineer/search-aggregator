namespace SearchAggregator.Api.Options;

public sealed class SearchEngineOptions
{
    public GoogleOptions Google { get; init; } = new();

    public WikipediaOptions Wikipedia { get; init; } = new();
}