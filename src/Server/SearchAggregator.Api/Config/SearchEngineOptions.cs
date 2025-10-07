namespace SearchAggregator.Api.Config;

public sealed class SearchEngineOptions
{
    public GoogleOptions Google { get; init; } = new();

    public sealed class GoogleOptions
    {
        public string ApiKey { get; init; } = string.Empty;
        public string SearchEngineId { get; init; } = string.Empty;
    }

    public sealed class WikipediaOptions
    {
        public string Endpoint { get; init; } = "https://en.wikipedia.org/w/api.php";
    }

    public WikipediaOptions Wikipedia { get; init; } = new();
}