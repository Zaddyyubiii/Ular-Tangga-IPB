import React from 'react';
import { motion } from 'framer-motion';

export default function TopHUD({ activePlayerName, activePlayerColor, timer }) {
  const isTimeLow = timer <= 3.5;
  const roundedTime = Math.ceil(timer);

  return (
    <div className="absolute top-4 left-0 right-0 flex justify-center z-10 select-none">
      <motion.div
        className="pointer-events-auto flex items-center gap-3 px-5 py-2.5 bg-game-dark/85 backdrop-blur-md rounded-cartoon border-4 border-white/10 shadow-bubble text-white font-playful"
        initial={{ y: -50, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        style={{
          boxShadow: isTimeLow 
            ? "0 0 25px rgba(230, 62, 39, 0.4), 0 6px 0 0 rgba(0, 0, 0, 0.25)" 
            : "0 6px 0 0 rgba(0, 0, 0, 0.25)",
          borderColor: isTimeLow ? "rgba(230, 62, 39, 0.6)" : "rgba(255, 255, 255, 0.1)"
        }}
        transition={{ type: "spring", stiffness: 200, damping: 15 }}
      >
        {/* Active Player Text */}
        <div className="flex flex-col">
          <span className="text-[10px] uppercase font-black text-slate-400 tracking-wider">Giliran Berjalan</span>
          <motion.span
            className="text-base font-extrabold"
            style={{ color: activePlayerColor }}
            animate={{ scale: [1, 1.05, 1] }}
            transition={{ duration: 2, repeat: Infinity }}
          >
            {activePlayerName ? activePlayerName.toUpperCase() : "MENUNGGU..."}
          </motion.span>
        </div>

        {/* Vertical Divider */}
        <div className="w-1 h-8 bg-slate-800 rounded-full" />

        {/* Cute Shake & Pulse Circle Timer */}
        <motion.div
          className={`flex items-center justify-center w-11 h-11 rounded-full border-4 font-black text-lg ${
            isTimeLow ? "bg-red-600 text-white border-white animate-pulse" : "bg-slate-900 text-yellow-300 border-slate-700"
          }`}
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
