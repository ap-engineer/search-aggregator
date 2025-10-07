import { useState } from "react";
import { Toaster } from "react-hot-toast";
import { useSearch } from "../hooks/useSearch";
import SearchInput from "../components/SearchInput";
import SubmitButton from "../components/SubmitButton";
import TotalHits from "../components/TotalHits";

export default function HomePage() {
    const [term, setTerm] = useState("");
    const { loading, result, handleSearch } = useSearch();

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();
        handleSearch(term);
    };

    return (
        <div className="flex flex-col items-center justify-center min-h-screen bg-gray-50">
            <Toaster position="top-right" />
            <TotalHits hits={result?.totalHits ?? null} />

            <form
                onSubmit={handleSubmit}
                className="flex flex-col sm:flex-row gap-3 w-11/12 sm:w-2/3 lg:w-1/2"
            >
                <SearchInput value={term} onChange={setTerm} />
                <SubmitButton loading={loading} />
            </form>
        </div>
    );
}
