interface Props {
    value: string;
    onChange: (val: string) => void;
    className?: string;
}

export default function SearchInput({ value, onChange, className = '' }: Props) {
    return (
        <input
            type="text"
            value={value}
            onChange={(e) => onChange(e.target.value)}
            placeholder="Search terms..."
            className={`flex-1 px-5 py-3.5 text-gray-800 bg-white/90 backdrop-blur-sm rounded-xl border-0 shadow-sm transition-all duration-200
                focus:ring-2 focus:ring-white/20 focus:bg-white focus:shadow-lg focus:outline-none
                hover:bg-white hover:shadow-md
                ${className}`}
        />
    );
}
