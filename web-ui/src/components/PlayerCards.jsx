import React from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const EVOLUTION_STAGES = [
  { name: "MABA 🌱", style: "bg-green-500/20 text-green-300 border-green-500/40" },
  { name: "SOPHOMORE 📖", style: "bg-blue-500/20 text-blue-300 border-blue-500/40" },
  { name: "JUNIOR 🔬", style: "bg-purple-500/20 text-purple-300 border-purple-500/40" },
  { name: "SENIOR 🎓", style: "bg-orange-500/20 text-orange-300 border-orange-500/40" },
  { name: "DUTA TATIB 👑", style: "bg-yellow-500/30 text-yellow-300 border-yellow-500 animate-pulse" }
];

export default function PlayerCards({ players, activePlayerId }) {
  if (!players || players.length === 0) return null;

  // Corner positioning maps for the 4 players
  const positionClasses = [
    "top-4 left-4",     // Player 1: Top-Left
    "top-4 right-4",    // Player 2: Top-Right
    "bottom-24 left-4", // Player 3: Bottom-Left (elevated above bottom bar)
    "bottom-24 right-4" // Player 4: Bottom-Right (elevated above bottom bar)
  ];

  return (
    <>
      {players.map((player, index) => {
        const isSelfActive = player.id === activePlayerId;
        const posClass = positionClasses[index] || "hidden";
        
        // Dynamic styling depending on state
        let borderGlow = "border-2 border-slate-700/50 shadow-bubble";
        if (isSelfActive) {
          borderGlow = `border-4 shadow-[0_0_20px_rgba(255,255,255,0.2)]`;
        }

        const currentEvo = EVOLUTION_STAGES[player.currentEvolutionStage] || EVOLUTION_STAGES[0];

        return (
          <motion.div
            key={player.id}
            className={`absolute ${posClass} w-64 p-3 rounded-cartoon pointer-events-auto bg-game-dark/85 backdrop-blur-md select-none border-4 ${borderGlow} overflow-hidden`}
            style={{ 
              borderColor: isSelfActive ? player.playerColorHex : "rgba(255, 255, 255, 0.15)",
              boxShadow: isSelfActive ? `0 8px 0 0 rgba(0, 0, 0, 0.35), 0 0 25px ${player.playerColorHex}40` : "0 6px 0 0 rgba(0, 0, 0, 0.3)"
            }}
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ 
              scale: isSelfActive ? 1.03 : 1.0, 
              opacity: player.isDroppedOut ? 0.5 : 1.0 
            }}
            transition={{ type: "spring", stiffness: 300, damping: 20 }}
          >
            {/* Active Turn Pulsating Banner */}
            <AnimatePresence>
              {isSelfActive && (
                <motion.div 
                  className="absolute top-0 right-0 left-0 h-1 bg-gradient-to-r from-transparent via-white/80 to-transparent"
                  animate={{ opacity: [0.3, 1, 0.3] }}
                  transition={{ duration: 1.5, repeat: Infinity }}
                />
              )}
            </AnimatePresence>

            {/* Header info: Name and Bot tag */}
            <div className="flex justify-between items-center mb-2">
              <span 
                className="font-extrabold text-lg truncate drop-shadow-[0_2px_2px_rgba(0,0,0,0.6)]"
                style={{ color: player.playerColorHex }}
              >
                {player.playerName}
              </span>
              <div className="flex gap-1.5 items-center">
                {player.isBot && (
                  <span className="text-[10px] font-bold px-1.5 py-0.5 rounded-full bg-slate-800 text-slate-300 border border-slate-700">
                    BOT 🤖
                  </span>
                )}
                {player.isDroppedOut && (
                  <span className="text-[10px] font-extrabold px-1.5 py-0.5 rounded-full bg-red-600 text-white border border-red-500 animate-pulse">
                    DO ❌
                  </span>
                )}
              </div>
            </div>

            {/* Grid statistics - Cartoon bubbly style */}
            <div className="grid grid-cols-2 gap-2 text-xs font-semibold mb-2">
              <div className="bg-slate-900/50 p-1.5 rounded-xl border border-slate-850 flex flex-col items-center">
                <span className="text-slate-400 text-[10px] uppercase font-bold">Ubin Aktif</span>
                <span className="text-sm font-extrabold text-yellow-300">
                  {player.isFinished ? "FINISH 🎉" : `# ${player.currentTile}`}
                </span>
              </div>
              <div className="bg-slate-900/50 p-1.5 rounded-xl border border-slate-850 flex flex-col items-center">
                <span className="text-slate-400 text-[10px] uppercase font-bold">Langgar Tatib</span>
                <span className="text-sm font-extrabold text-red-400 flex items-center gap-1">
                  💀 {player.skullHitCount}
                </span>
              </div>
            </div>

            {/* Footer status row */}
            <div className="flex justify-between items-center mt-2.5 pt-2 border-t border-slate-800/80">
              {/* Evolution Rank Badge */}
              <span className={`text-[10px] font-extrabold px-2 py-0.5 rounded-lg border ${currentEvo.style}`}>
                {currentEvo.name}
              </span>

              {/* Status String */}
              <div className="text-right">
                {player.skipTurns > 0 ? (
                  <span className="text-[10px] bg-amber-500/20 text-amber-300 border border-amber-500/30 px-1.5 py-0.5 rounded-lg font-bold animate-pulse">
                    DISKORS ({player.skipTurns} giliran)
                  </span>
                ) : (
                  <span 
                    className="text-[10px] font-extrabold uppercase"
                    style={{ color: isSelfActive ? player.playerColorHex : "#94a3b8" }}
                  >
                    {player.status}
                  </span>
                )}
              </div>
            </div>

            {/* Pulsating active arrow */}
            {isSelfActive && (
              <motion.div 
                className="absolute top-1 right-2"
                animate={{ y: [0, -3, 0] }}
                transition={{ duration: 1.2, repeat: Infinity, ease: "easeInOut" }}
              >
                <div 
                  className="w-2.5 h-2.5 rounded-full"
                  style={{ backgroundColor: player.playerColorHex }}
                />
              </motion.div>
            )}
          </motion.div>
        );
      })}
    </>
  );
}
