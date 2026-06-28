import { motion, AnimatePresence } from 'framer-motion';
import PlayerSprite from './PlayerSprite';

const EVOLUTION_STAGES = [
  { name: "MABA 🌱", style: "wood-panel text-[var(--color-game-cream-text)]" },
  { name: "SOPHOMORE 📖", style: "wood-panel text-[var(--color-game-cream-text)]" },
  { name: "JUNIOR 🔬", style: "wood-panel text-[var(--color-game-cream-text)]" },
  { name: "SENIOR 🎓", style: "wood-panel text-[var(--color-game-cream-text)]" },
  { name: "DUTA TATIB 👑", style: "wood-panel text-yellow-300 border-yellow-500 animate-pulse" }
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
        // Dynamic styling depending on state

        const currentEvo = EVOLUTION_STAGES[player.currentEvolutionStage] || EVOLUTION_STAGES[0];

        return (
          <motion.div
            key={player.id}
            className={`absolute ${posClass} w-64 p-3 rounded-xl pointer-events-auto parchment-panel select-none overflow-hidden transition-transform ${isSelfActive ? 'z-20' : 'z-10'}`}
            style={{ 
              borderColor: 'var(--color-game-dark-wood)',
              backgroundColor: isSelfActive ? 'var(--color-game-parchment-light)' : 'var(--color-game-parchment)',
              boxShadow: isSelfActive ? `0 0 0 4px ${player.playerColorHex}, var(--shadow-pixel)` : "var(--shadow-pixel)"
            }}
            initial={{ scale: 0.9, opacity: 0 }}
            animate={{ 
              scale: isSelfActive ? 1.05 : 1.0, 
              opacity: player.isDroppedOut ? 0.5 : 1.0 
            }}
            transition={{ type: "spring", stiffness: 300, damping: 20 }}
          >
            {/* Active Turn Pulsating Banner */}
            <AnimatePresence>
              {isSelfActive && (
                <motion.div 
                  className="absolute top-0 right-0 left-0 text-center font-bangers text-[10px] tracking-widest text-[var(--color-game-cream-text)] py-0.5 z-10"
                  style={{ backgroundColor: player.playerColorHex }}
                  initial={{ opacity: 0, y: -10 }}
                  animate={{ opacity: 1, y: 0 }}
                  exit={{ opacity: 0, y: -10 }}
                >
                  GILIRAN BERJALAN
                </motion.div>
              )}
            </AnimatePresence>

            {/* Header info: Name and Bot tag */}
            <div className={`flex justify-between items-center mb-2 ${isSelfActive ? 'mt-4' : ''}`}>
              <div className="flex items-center gap-2">
                <div className="w-12 h-12 rounded-full wood-bg border-2 flex items-center justify-center overflow-hidden" style={{ borderColor: player.playerColorHex }}>
                  <PlayerSprite hexColor={player.playerColorHex} stage={player.currentEvolutionStage} currentTile={player.currentTile} />
                </div>
                <span
                  className="font-black text-xl truncate drop-shadow-sm font-playful tracking-wide"
                  style={{
                    color: player.playerColorHex,
                    WebkitTextStroke: '1px var(--color-game-dark-wood)'
                  }}
                >
                  {player.playerName}
                </span>
              </div>
              <div className="flex gap-1.5 items-center">
                {player.isBot && (
                  <span className="text-[10px] font-bangers px-2 py-0.5 rounded border border-[var(--color-game-dark-wood)] wood-bg text-[var(--color-game-cream-text)]">
                    BOT
                  </span>
                )}
                {player.isDroppedOut && (
                  <span className="text-[10px] font-bangers px-2 py-0.5 rounded bg-red-600 text-white border border-red-800 animate-pulse">
                    DO ❌
                  </span>
                )}
              </div>
            </div>

            {/* Grid statistics - Wood Panel Style */}
            <div className="grid grid-cols-2 gap-2 text-xs font-semibold mb-2">
              <div className="wood-bg text-[var(--color-game-cream-text)] p-1.5 rounded-lg border-2 border-[var(--color-game-dark-wood)] flex flex-col items-center">
                <span className="text-[10px] uppercase font-bold opacity-80">Ubin Aktif</span>
                <span className="text-sm font-black text-yellow-300">
                  {player.isFinished ? "FINISH 🎉" : `# ${player.currentTile}`}
                </span>
              </div>
              <div className="wood-bg text-[var(--color-game-cream-text)] p-1.5 rounded-lg border-2 border-[var(--color-game-dark-wood)] flex flex-col items-center">
                <span className="text-[10px] uppercase font-bold opacity-80">Langgar Tatib</span>
                <span className="text-sm font-black text-red-400 flex items-center gap-1">
                  💀 {player.skullHitCount}
                </span>
              </div>
            </div>

            {/* Footer status row */}
            <div className="flex justify-between items-center mt-2.5 pt-2 border-t-2 border-[var(--color-game-dark-wood)] border-dashed opacity-80">
              {/* Evolution Rank Badge */}
              <span className={`text-[10px] font-bangers px-2 py-0.5 rounded-md border-2 ${currentEvo.style}`}>
                {currentEvo.name}
              </span>

              {/* Status String */}
              <div className="text-right">
                {player.skipTurns > 0 ? (
                  <span className="text-[10px] bg-red-500 text-white border border-red-800 px-1.5 py-0.5 rounded-lg font-bold animate-pulse">
                    DISKORS ({player.skipTurns} giliran)
                  </span>
                ) : (
                  <span 
                    className="text-[12px] font-bangers uppercase drop-shadow-sm"
                    style={{ color: isSelfActive ? player.playerColorHex : "var(--color-game-dark-text)" }}
                  >
                    {player.status}
                  </span>
                )}
              </div>
            </div>

            {/* Pulsating active arrow */}
            {isSelfActive && (
              <motion.div 
                className="absolute bottom-2 right-2 flex justify-center items-center"
                animate={{ y: [0, -3, 0] }}
                transition={{ duration: 1.2, repeat: Infinity, ease: "easeInOut" }}
              >
                <div 
                  className="w-3 h-3 rounded-sm rotate-45 border-2 border-white"
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
