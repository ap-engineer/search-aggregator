#!/bin/bash

# Test script for Search Aggregator API
# This script tests the API endpoints once the application is running

BASE_URL="http://localhost:5000"

echo "🔍 Search Aggregator API Test Script"
echo "======================================"

# Test health endpoint
echo ""
echo "1. Testing health endpoint..."
curl -s "$BASE_URL/health" | jq '.' || echo "Health endpoint failed or jq not installed"

# Test search endpoint with simple query
echo ""
echo "2. Testing search endpoint with 'hello'..."
curl -s "$BASE_URL/api/search?query=hello" | jq '.' || echo "Search endpoint failed or jq not installed"

# Test search endpoint with multi-word query
echo ""
echo "3. Testing search endpoint with 'hello world'..."
curl -s "$BASE_URL/api/search?query=hello%20world" | jq '.' || echo "Search endpoint failed or jq not installed"

# Test search endpoint with empty query (should fail)
echo ""
echo "4. Testing search endpoint with empty query (should return 400)..."
curl -s -w "HTTP Status: %{http_code}\n" "$BASE_URL/api/search?query=" || echo "Empty query test failed"

echo ""
echo "✅ Test script completed!"
echo "Note: Make sure the API is running on $BASE_URL before running this script"
echo "To start the API: cd src/Server/SearchAggregator.Api && dotnet run"
