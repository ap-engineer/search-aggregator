import axios from "axios";
import type {SearchResponse} from "../types/search";

const API_BASE = import.meta.env.VITE_API_BASE;
export const search = async (term: string): Promise<SearchResponse> => {
    const response = await axios.get<SearchResponse>(API_BASE, {
        params: {term},
    });
    return response.data;
}