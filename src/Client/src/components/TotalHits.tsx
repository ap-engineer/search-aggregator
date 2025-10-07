import { motion, AnimatePresence } from "framer-motion";

interface Props {
    hits: number | null;
}

export default function TotalHits({ hits }: Props) {
    if (hits === null) return null;
    
    return (
        <AnimatePresence>
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0 }}
                transition={{ duration: 0.3, ease: "easeOut" }}
                className="text-center mb-6"
            >
                <div className="inline-flex items-center px-4 py-2.5 rounded-full bg-white/5 backdrop-blur-sm border border-white/10">
                    <span className="text-sm font-medium text-indigo-200">
                        Found <span className="font-bold text-white">{hits.toLocaleString()}</span> results
                    </span>
                </div>
            </motion.div>
        </AnimatePresence>
    );
}
