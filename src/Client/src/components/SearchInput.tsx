interface Props {
    value: string;
    onChange: (val: string) => void;
}

export default function SearchInput({ value, onChange }: Props) {
    return (
        <input
            type="text"
            value={value}
            onChange={(e) => onChange(e.target.value)}
            placeholder="Enter your search terms..."
            className="flex-1 p-3 rounded-lg border border-gray-300 focus:outline-none focus:ring-2 focus:ring-blue-500 text-lg"
        />
    );
}
