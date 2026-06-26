import React from 'react';
import { motion } from 'framer-motion';

export default function TopHUD({ activePlayerName, activePlayerColor, timer }) {
  const isTimeLow = timer <= 3.5;
  const roundedTime = Math.ceil(timer);

  return (
    <div className="absolute top-4 left-0 right-0 flex justify-center z-10 select-none">
      <motion.div
        className="pointer-events-auto flex items-center gap-4 px-6 py-2 wood-panel"
        initial={{ y: -50, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        style={{
          boxShadow: isTimeLow 
            ? "0 0 25px rgba(230, 62, 39, 0.6), var(--shadow-pixel)" 
            : "var(--shadow-pixel)",
          borderColor: isTimeLow ? "#E84C4C" : "var(--color-game-dark-wood)"
        }}
        transition={{ type: "spring", stiffness: 200, damping: 15 }}
      >
        {/* Active Player Text */}
        <div className="flex flex-col">
          <span className="text-[12px] font-bangers text-[var(--color-game-parchment-light)] tracking-widest uppercase">Giliran Berjalan</span>
          <motion.span
            className="text-xl font-black drop-shadow-md"
            style={{ color: activePlayerColor || 'var(--color-game-cream-text)' }}
            animate={{ scale: [1, 1.05, 1] }}
            transition={{ duration: 2, repeat: Infinity }}
          >
            {activePlayerName ? activePlayerName.toUpperCase() : "MENUNGGU..."}
          </motion.span>
        </div>

        {/* Vertical Divider */}
        <div className="w-1 h-10 bg-[var(--color-game-dark-wood)] rounded-full opacity-80" />

        {/* Timer */}
        <motion.div
          className="flex items-center justify-center w-12 h-12 rounded-full border-4 font-bangers text-2xl"
          style={{
            backgroundColor: isTimeLow ? '#E84C4C' : 'var(--color-game-parchment)',
            color: isTimeLow ? 'white' : 'var(--color-game-dark-text)',
            borderColor: isTimeLow ? 'white' : 'var(--color-game-dark-wood)',
          }}
          animate={isTimeLow ? {
            x: [0, -3, 3, -3, 3, 0],
            rotate: [0, -2, 2, -2, 2, 0]
          } : {}}
          transition={{ duration: 0.25, repeat: isTimeLow ? Infinity : 0 }}
        >
          {roundedTime}
        </motion.div>
      </motion.div>
    </div>
  );
}
