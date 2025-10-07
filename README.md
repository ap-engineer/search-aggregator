# Search Aggregator API

A .NET 9 web API that aggregates search results from multiple search engines (Google, Wikipedia, and could others) and returns the total number of hits for each search term.

## Features

- **Multi-Engine Search**: Searches across Google and Wiki
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
GET /api/search?query=Hello
```

**Example Response:**
```json
{
  "query": "Hello world",
  "totalHits": 22222222,
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
- Node.js /w npm
- Or ~~Docker~~ (for containerized deployment)

### Using .NET CLI
```bash
cd src/Server/SearchAggregator.Api
dotnet restore
dotnet run
```

## Accessing the API

Once running, you can access:
- **Swagger UI**: `https://localhost:5001/swagger/index.html`
- **API Endpoint**: `http://localhost:5001/api/search?query=your+search+terms`
- **Health Check**: `http://localhost:5001/health`
