import React from 'react';
import { motion } from 'framer-motion';
import { useAudio } from '../hooks/useAudio';

export default function GameOverModal({ winnerName, winnerColor, message, onPlayAgain, onReturnMenu }) {
  const { playSfx } = useAudio();
  React.useEffect(() => {
    const handleKeyDown = (event) => {
      if (event.key === 'Enter') {
        event.preventDefault();
        playSfx('click');
        onPlayAgain();
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [onPlayAgain, playSfx]);

  return (
    <div className="absolute inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4 pointer-events-auto select-none font-playful">
      <motion.div
        className="w-full max-w-md parchment-panel shadow-2xl p-6 text-center"
        style={{
          boxShadow: `0 0 40px ${winnerColor || "var(--color-game-deep-grass)"}60, var(--shadow-pixel)`,
          borderColor: "var(--color-game-dark-wood)"
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
        <h1 className="text-3xl font-bangers mb-1 text-[var(--color-game-dark-text)] tracking-wide uppercase drop-shadow-sm">
          Duta Tatib Terpilih!
        </h1>
        <div className="w-16 h-1 bg-[var(--color-game-dark-wood)] mx-auto rounded-full mb-5 opacity-40" />

        {/* Winner Announcement Card */}
        <motion.div
          className="wood-panel p-5 rounded-2xl mb-6 flex flex-col items-center gap-1.5"
          initial={{ y: 20, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.2 }}
        >
          <span className="text-[12px] uppercase font-bangers text-[var(--color-game-parchment-light)] tracking-wider">Sang Juara</span>
          <motion.span
            className="text-3xl font-black tracking-wide drop-shadow-md"
            style={{ color: winnerColor || "var(--color-game-cream-text)" }}
            animate={{ scale: [1, 1.06, 1] }}
            transition={{ duration: 1.5, repeat: Infinity }}
          >
            👑 {winnerName ? winnerName.toUpperCase() : "PLAYER"} 👑
          </motion.span>
        </motion.div>

        {/* Congratulations message */}
        <p className="text-sm font-bold text-[var(--color-game-dark-text)] leading-relaxed px-2 mb-6 font-playful">
          {message || "Selamat! Kamu berhasil melewati semua tantangan, mematuhi tata tertib, dan dinobatkan menjadi Duta Tata Tertib IPB University!"}
        </p>

        {/* Bubbly actions row */}
        <div className="flex gap-3">
          <button
            onClick={() => {
              playSfx('click');
              onPlayAgain();
            }}
            className="flex-1 py-3 px-4 rounded-xl wood-button bg-[var(--color-game-grass)] hover:bg-[var(--color-game-soft-grass)] text-xl transition-all duration-100"
            style={{ borderColor: 'var(--color-game-deep-grass)', backgroundColor: 'var(--color-game-grass)' }}
          >
            Main Lagi 🎮
          </button>
          
          <button
            onClick={() => {
              playSfx('click');
              onReturnMenu();
            }}
            className="flex-1 py-3 px-4 rounded-xl wood-button bg-[var(--color-game-wood)] hover:bg-[var(--color-game-light-dirt)] text-[var(--color-game-dark-text)] text-xl transition-all duration-100"
            style={{ borderColor: 'var(--color-game-dark-wood)', backgroundColor: 'var(--color-game-dirt)' }}
          >
            Menu Utama 🏠
          </button>
        </div>
      </motion.div>
    </div>
  );
}
