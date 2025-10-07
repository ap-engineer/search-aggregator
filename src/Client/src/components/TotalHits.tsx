import { motion, AnimatePresence } from "framer-motion";

interface Props {
    hits: number | null;
}

export default function TotalHits({ hits }: Props) {
    return (
        <AnimatePresence>
            {hits !== null && (
                <motion.h1
                    key="hits"
                    initial={{ opacity: 0, y: -20 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0 }}
                    transition={{ duration: 0.4 }}
                    className="text-4xl font-bold text-gray-800 mb-8"
                >
                    {hits.toLocaleString()} total hits
                </motion.h1>
            )}
        </AnimatePresence>
    );
}
