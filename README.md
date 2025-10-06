# Search Aggregator API

A .NET 9 web API that aggregates search results from multiple search engines (Google, Bing, and DuckDuckGo) and returns the total number of hits for each search term.

## Features

- **Multi-Engine Search**: Searches across Google, Bing, and DuckDuckGo
- **Word-by-Word Analysis**: Splits queries into individual words and searches each separately
- **Result Aggregation**: Sums up hit counts for each word per search engine
- **Swagger Documentation**: Interactive API documentation available at the root URL
- **Robust Error Handling**: Graceful handling of search engine failures
- **Structured Logging**: Comprehensive logging with Serilog
- **Clean Architecture**: Well-organized code with dependency injection

## API Endpoints

### GET /api/search
Searches across multiple search engines and returns aggregated hit counts.

**Parameters:**
- `query` (required): The search query (can contain multiple words)

**Example Request:**
```
GET /api/search?query=Hello world
```

**Example Response:**
```json
{
  "query": "Hello world",
  "searchTerms": ["Hello", "world"],
  "searchEngines": [
    {
      "name": "Google",
      "totalHits": 154000000,
      "isSuccess": true,
      "errorMessage": null
    },
    {
      "name": "Bing",
      "totalHits": 89000000,
      "isSuccess": true,
      "errorMessage": null
    },
    {
      "name": "DuckDuckGo",
      "totalHits": 45000000,
      "isSuccess": true,
      "errorMessage": null
    }
  ],
  "searchedAt": "2024-01-15T10:30:00Z",
  "totalSearchTimeMs": 2500.5,
  "hasErrors": false
}
```

### GET /health
Health check endpoint for monitoring service availability.

## How It Works

1. **Query Processing**: The input query is split into individual search terms
2. **Parallel Search**: Each search engine searches for each term simultaneously
3. **Result Aggregation**: Hit counts for all terms are summed per search engine
4. **Response Formation**: Results are formatted and returned with metadata

For example, if you search for "Hello world":
- Google searches for "Hello" (54M hits) + "world" (100M hits) = 154M total
- Bing searches for "Hello" (30M hits) + "world" (59M hits) = 89M total
- DuckDuckGo searches for "Hello" (20M hits) + "world" (25M hits) = 45M total

## Running the Application

### Prerequisites
- .NET 9 SDK
- Or Docker (for containerized deployment)

### Using .NET CLI
```bash
cd src/Server/SearchAggregator.Api
dotnet restore
dotnet run
```

### Using Docker
```bash
cd src/Server/SearchAggregator.Api
docker build -t search-aggregator .
docker run -p 8080:8080 search-aggregator
```

### Using Docker Compose
```bash
cd src/Server/SearchAggregator.Api
docker-compose up
```

## Accessing the API

Once running, you can access:
- **Swagger UI**: `http://localhost:5000` (or `http://localhost:8080` for Docker)
- **API Endpoint**: `http://localhost:5000/api/search?query=your+search+terms`
- **Health Check**: `http://localhost:5000/health`

## Architecture

The application follows clean architecture principles:

```
├── Controllers/           # API controllers
├── DTOs/                 # Data transfer objects
├── Models/               # Domain models
├── Services/
│   ├── Interfaces/       # Service contracts
│   ├── SearchEngines/    # Search engine implementations
│   └── SearchAggregatorService.cs
└── Program.cs           # Application startup and configuration
```

## Search Engines

### Google Search Engine
- Scrapes Google search results pages
- Parses result statistics from the results info div
- Handles various Google result page formats

### Bing Search Engine
- Scrapes Bing search results pages
- Extracts hit counts from result statistics
- Supports multiple Bing result count formats

### DuckDuckGo Search Engine
- Scrapes DuckDuckGo HTML search results
- Estimates total results based on visible results
- Provides conservative hit count estimates

## Configuration

Key configuration options in `appsettings.json`:

```json
{
  "SearchEngines": {
    "RequestTimeout": "00:00:30",
    "UserAgent": "Mozilla/5.0 ..."
  }
}
```

## Error Handling

- Individual search engine failures don't break the entire request
- Failed searches are marked with `isSuccess: false` and include error messages
- The API continues to return results from successful search engines
- Comprehensive logging helps with debugging issues

## Security Considerations

- Uses proper User-Agent headers to avoid being blocked
- Implements request timeouts to prevent hanging requests
- No API keys required (uses public search interfaces)
- Rate limiting should be implemented for production use

## Future Enhancements

- Add more search engines (Yahoo, Yandex, etc.)
- Implement caching for frequently searched terms
- Add rate limiting and request throttling
- Support for phrase searches (quoted terms)
- Add search result filtering and sorting options
