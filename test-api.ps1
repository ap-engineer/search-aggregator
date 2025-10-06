# PowerShell Test script for Search Aggregator API
# This script tests the API endpoints once the application is running

$BaseUrl = "http://localhost:5000"

Write-Host "🔍 Search Aggregator API Test Script" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan

# Test health endpoint
Write-Host ""
Write-Host "1. Testing health endpoint..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/health" -Method Get
    $response | ConvertTo-Json -Depth 3
}
catch {
    Write-Host "Health endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test search endpoint with simple query
Write-Host ""
Write-Host "2. Testing search endpoint with 'hello'..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/search?query=hello" -Method Get
    $response | ConvertTo-Json -Depth 3
}
catch {
    Write-Host "Search endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test search endpoint with multi-word query
Write-Host ""
Write-Host "3. Testing search endpoint with 'hello world'..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/search?query=hello%20world" -Method Get
    $response | ConvertTo-Json -Depth 3
}
catch {
    Write-Host "Search endpoint failed: $($_.Exception.Message)" -ForegroundColor Red
}

# Test search endpoint with empty query (should fail)
Write-Host ""
Write-Host "4. Testing search endpoint with empty query (should return 400)..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$BaseUrl/api/search?query=" -Method Get
    Write-Host "Unexpected success with empty query" -ForegroundColor Red
}
catch {
    Write-Host "Expected error with empty query: $($_.Exception.Message)" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ Test script completed!" -ForegroundColor Green
Write-Host "Note: Make sure the API is running on $BaseUrl before running this script" -ForegroundColor Yellow
Write-Host "To start the API: cd src/Server/SearchAggregator.Api && dotnet run" -ForegroundColor Yellow
