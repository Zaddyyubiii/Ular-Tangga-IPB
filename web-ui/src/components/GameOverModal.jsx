import React from 'react';
import { motion } from 'framer-motion';

export default function GameOverModal({ winnerName, winnerColor, message, onPlayAgain, onReturnMenu }) {
  return (
    <div className="absolute inset-0 bg-slate-950/90 backdrop-blur-md z-50 flex items-center justify-center p-4 pointer-events-auto select-none font-playful">
      <motion.div
        className="w-full max-w-md bg-game-dark/95 border-4 border-slate-700/60 rounded-cartoon p-6 text-center text-white shadow-bubble"
        style={{
          boxShadow: `0 0 40px ${winnerColor || "#e63e27"}40, 0 8px 0 0 rgba(0,0,0,0.5)`,
          borderColor: winnerColor ? `${winnerColor}80` : "rgba(255, 255, 255, 0.15)"
        }}
        initial={{ scale: 0.7, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        exit={{ scale: 0.7, opacity: 0 }}
        transition={{ type: "spring", stiffness: 250, damping: 18 }}
      >
        {/* Animated Trophy Icon */}
        <motion.div
          className="text-6xl mb-4 inline-block"
          animate={{ rotate: [0, -10, 10, -10, 10, 0] }}
          transition={{ duration: 1.5, repeat: Infinity, repeatDelay: 1 }}
        >
          🏆
        </motion.div>

        {/* Victory Header */}
        <h1 className="text-2xl font-black mb-1 text-slate-100 tracking-wide uppercase">
          Duta Tatib Terpilih!
        </h1>
        <div className="w-16 h-1 bg-yellow-400 mx-auto rounded-full mb-5" />

        {/* Winner Announcement Card */}
        <motion.div
          className="bg-slate-900/80 p-5 rounded-2xl border border-slate-800/80 mb-6 flex flex-col items-center gap-1.5"
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.2 }}
        >
          <span className="text-[10px] uppercase font-extrabold text-slate-400">Sang Juara</span>
          <motion.span
            className="text-3xl font-black tracking-wide drop-shadow-[0_2px_4px_rgba(0,0,0,0.6)]"
            style={{ color: winnerColor || "#ffffff" }}
            animate={{ scale: [1, 1.06, 1] }}
            transition={{ duration: 1.5, repeat: Infinity }}
          >
            👑 {winnerName ? winnerName.toUpperCase() : "PLAYER"} 👑
          </motion.span>
        </motion.div>

        {/* Congratulations message */}
        <p className="text-xs font-semibold text-slate-300 leading-relaxed px-2 mb-6">
          {message || "Selamat! Kamu berhasil melewati semua tantangan, mematuhi tata tertib, dan dinobatkan menjadi Duta Tata Tertib IPB University!"}
        </p>

        {/* Bubbly actions row */}
        <div className="flex gap-3">
          <button
            onClick={onPlayAgain}
            className="flex-1 py-3 px-4 rounded-xl bg-green-600 hover:bg-green-500 text-white font-black text-xs uppercase cursor-pointer border-b-4 border-green-800 active:border-b-0 active:translate-y-1 transition-all duration-100"
            style={{
              boxShadow: "0 4px 0 0 rgba(22, 101, 52, 1)"
            }}
          >
            Main Lagi 🎮
          </button>
          
          <button
            onClick={onReturnMenu}
            className="flex-1 py-3 px-4 rounded-xl bg-slate-700 hover:bg-slate-650 text-slate-200 font-black text-xs uppercase cursor-pointer border-b-4 border-slate-900 active:border-b-0 active:translate-y-1 transition-all duration-100"
            style={{
              boxShadow: "0 4px 0 0 rgba(30, 41, 59, 1)"
            }}
          >
            Menu Utama 🏠
          </button>
        </div>
      </motion.div>
    </div>
  );
}
