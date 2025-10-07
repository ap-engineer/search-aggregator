import { useState } from "react";
import { search } from "../api/searchApi";
import type {SearchResponse} from "../types/search";
import toast from "react-hot-toast";

export function useSearch() {
    const [loading, setLoading] = useState(false);
    const [result, setResult] = useState<SearchResponse | null>(null);

    const handleSearch = async (term: string) => {
        if (!term.trim()) {
            toast.error("Please enter a search term.");
            return;
        }

        try {
            setLoading(true);
            setResult(null);
            const res = await search(term);
            setResult(res);
            toast.success("Search completed!");
        } catch (error) {
            console.error(error);
            toast.error("Failed to perform search.");
        } finally {
            setLoading(false);
        }
    };

    return { loading, result, handleSearch };
}
