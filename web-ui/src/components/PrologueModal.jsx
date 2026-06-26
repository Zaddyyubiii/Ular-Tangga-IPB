import React from 'react';
import { motion } from 'framer-motion';

export default function PrologueModal({ text, onStart }) {
  React.useEffect(() => {
    if (!text) return;

    const handleKeyDown = (event) => {
      if (event.key === 'Enter') {
        event.preventDefault();
        onStart();
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [text, onStart]);

  if (!text) return null;

  return (
    <div className="absolute inset-0 bg-slate-950/85 backdrop-blur-md z-50 flex items-center justify-center p-4 pointer-events-auto select-none font-playful">
      <motion.div
        className="w-full max-w-xl parchment-panel rounded-2xl p-6 text-center shadow-bubble"
        style={{
          boxShadow: "var(--shadow-pixel)",
          borderColor: "var(--color-game-dark-wood)",
          backgroundColor: "var(--color-game-parchment-light)"
        }}
        initial={{ y: 50, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        exit={{ y: -50, opacity: 0 }}
        transition={{ type: "spring", stiffness: 200, damping: 15 }}
      >
        {/* Animated decorative scroll graphic */}
        <div className="text-5xl mb-4 animate-bounce">
          📜
        </div>

        {/* Narrative Title */}
        <h1 
          className="text-3xl font-pixel font-bold mb-4 tracking-wider"
          style={{ 
            color: "var(--color-game-dark-wood)", 
            WebkitTextStroke: "1px var(--color-game-parchment-light)"
          }}
        >
          PETUALANGAN DIMULAI!
        </h1>

        {/* Narrator Container */}
        <div className="wood-bg border-4 border-[var(--color-game-dark-wood)] p-5 rounded-xl mb-6 max-h-[220px] overflow-y-auto">
          <p className="text-sm font-bold text-[var(--color-game-cream-text)] leading-relaxed text-justify whitespace-pre-line px-1 font-playful tracking-wide">
            {text}
          </p>
        </div>

        {/* Start Button */}
        <button
          onClick={onStart}
          className="w-full py-4 px-6 rounded-xl wood-button text-[var(--color-game-cream-text)] font-pixel text-xl uppercase tracking-widest hover:scale-[1.02] transition-transform duration-150"
        >
          Mulai Perjalanan 🚀
        </button>
      </motion.div>
    </div>
  );
}
