import React from 'react';
import { motion } from 'framer-motion';

export default function PrologueModal({ text, onStart }) {
  if (!text) return null;

  return (
    <div className="absolute inset-0 bg-slate-950/85 backdrop-blur-md z-50 flex items-center justify-center p-4 pointer-events-auto select-none font-playful">
      <motion.div
        className="w-full max-w-xl bg-game-dark/95 border-4 border-slate-700/60 rounded-cartoon p-6 text-center text-white relative shadow-bubble"
        style={{
          boxShadow: "0 0 30px rgba(0, 98, 255, 0.25), 0 8px 0 0 rgba(0, 0, 0, 0.45)",
          borderColor: "rgba(255, 255, 255, 0.15)"
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
        <h1 className="text-2xl font-black mb-4 text-yellow-300 drop-shadow-[0_2px_4px_rgba(0,0,0,0.6)]">
          PETUALANGAN DIMULAI!
        </h1>

        {/* Narrator Container */}
        <div className="bg-slate-900/60 border border-slate-800 p-5 rounded-2xl mb-6 max-h-[220px] overflow-y-auto">
          <p className="text-sm font-semibold text-slate-200 leading-relaxed text-justify whitespace-pre-line px-1">
            {text}
          </p>
        </div>

        {/* Start Button */}
        <button
          onClick={onStart}
          className="w-full py-3.5 px-6 rounded-xl bg-green-600 hover:bg-green-500 text-white font-black text-sm uppercase cursor-pointer border-b-6 border-green-800 active:border-b-0 active:translate-y-1 transition-all duration-100 hover:scale-[1.02]"
          style={{
            boxShadow: "0 6px 0 0 rgba(22, 101, 52, 1)"
          }}
        >
          Mulai Perjalanan 🚀
        </button>
      </motion.div>
    </div>
  );
}
