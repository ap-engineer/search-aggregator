import { type FormEvent, useState } from "react";
import { Toaster } from "react-hot-toast";
import { useSearch } from "../hooks/useSearch";
import SearchInput from "../components/SearchInput";
import SubmitButton from "../components/SubmitButton";
import TotalHits from "../components/TotalHits";

const HomePage = () => {
    const [term, setTerm] = useState("");
    const { loading, result, handleSearch } = useSearch();

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        if (!term.trim()) return;
        await handleSearch(term);
    };

    return (
        <div className="min-h-screen flex items-center justify-center p-4 bg-gradient-to-br from-indigo-900 via-purple-900 to-gray-900">
            <div className="w-full max-w-2xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="text-center mb-10">
                    <h1 className="text-4xl font-bold text-white mb-2">Search Aggregator</h1>
                    <p className="text-indigo-200">Find what you're looking for across multiple search engines</p>
                </div>

                <div className="space-y-6">
                    <TotalHits hits={result?.totalHits ?? null} />
                    
                    <form onSubmit={handleSubmit} className="space-y-4">
                        <div className="flex flex-col sm:flex-row gap-3 w-full">
                            <SearchInput 
                                value={term} 
                                onChange={setTerm}
                                className="flex-1"
                            />
                            <SubmitButton loading={loading} />
                        </div>
                    </form>
                </div>
            </div>

            <Toaster 
                position="bottom-center"
                toastOptions={{
                    style: {
                        background: 'rgba(255, 255, 255, 0.1)',
                        backdropFilter: 'blur(10px)',
                        color: '#fff',
                        borderRadius: '12px',
                        border: '1px solid rgba(255, 255, 255, 0.1)',
                        padding: '16px',
                        fontSize: '0.95rem',
                    },
                    duration: 4000,
                }}
            />
        </div>
    );
};

export default HomePage;