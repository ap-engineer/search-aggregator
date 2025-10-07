interface Props {
    loading: boolean;
}

const SubmitButton = ({loading}: Props) => (
    <button
        type="submit"
        disabled={loading}
        className="px-6 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 disabled:bg-gray-400 transition-colors"
    >
        {loading ? "Searching..." : "Submit"}
    </button>
);

export default SubmitButton;