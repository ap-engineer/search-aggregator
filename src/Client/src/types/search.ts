export interface SearchEngineResult {
    engine: string;
    totalHits: number;
    isSuccess: boolean;
    error?: string | null;
}

export interface SearchResponse {
    query: string;
    totalHits: number;
    searchedAt: string;
    totalSearchTimeMs: number;
    hasErrors: boolean;
    searchEngines: SearchEngineResult[];
}
