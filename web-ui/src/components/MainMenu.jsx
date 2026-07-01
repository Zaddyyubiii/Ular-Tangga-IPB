import { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { useAudio } from '../hooks/useAudio';

const MainMenu = ({ onStartGame }) => {
  const { playSfx } = useAudio();
  const [playerCount, setPlayerCount] = useState(4);
  const [playerNames, setPlayerNames] = useState(["", "", "", ""]);

  const handleNameChange = (index, value) => {
    const newNames = [...playerNames];
    newNames[index] = value;
    setPlayerNames(newNames);
  };

  const stopProp = (e) => {
    e.stopPropagation();
    if (e.nativeEvent && e.nativeEvent.stopImmediatePropagation) {
      e.nativeEvent.stopImmediatePropagation();
    }
  };

  const handleStart = () => {
    playSfx('click');
    onStartGame({ playerCount, playerNames });
  };

  return (
    <div className="absolute inset-0 flex items-center justify-center pointer-events-auto select-text bg-[var(--color-game-sky)] font-playful">
      {/* Background decoration: Grass and path */}
      <div className="absolute bottom-0 w-full h-[40%] bg-[var(--color-game-grass)] border-t-[12px] border-[var(--color-game-deep-grass)] flex justify-center">
        <div className="w-1/3 h-full bg-[var(--color-game-dirt)] border-x-[8px] border-[var(--color-game-wood)] opacity-80"></div>
      </div>
      
      <motion.div 
        initial={{ y: -60, opacity: 0 }}
        animate={{ y: 0, opacity: 1 }}
        transition={{ type: "spring", bounce: 0.5, duration: 0.8 }}
        className="parchment-panel relative z-10 flex flex-col items-center p-8 sm:p-10 min-w-[320px] sm:min-w-[420px]"
      >
        <div className="absolute -top-10 flex gap-2">
            <motion.div animate={{ rotate: 360 }} transition={{ duration: 10, repeat: Infinity, ease: "linear" }} className="text-4xl drop-shadow-md">🎲</motion.div>
            <motion.div animate={{ rotate: -360 }} transition={{ duration: 12, repeat: Infinity, ease: "linear" }} className="text-4xl drop-shadow-md">🎲</motion.div>
        </div>

        <div className="wood-panel px-10 py-5 mb-8 text-center border-[5px]">
          <h1 className="text-3xl sm:text-4xl font-pixel font-bold tracking-widest text-[var(--color-game-cream-text)] drop-shadow-md uppercase">Ular Tangga</h1>
          <h2 className="text-xl sm:text-2xl font-pixel font-bold tracking-wider text-[var(--color-game-cream-text)] mt-1 uppercase">Tata Tertib IPB</h2>
        </div>

        <div className="flex items-center justify-center gap-6 mb-8 w-full">
          <motion.button 
            whileTap={{ scale: 0.9 }}
            onClick={() => {
              playSfx('click');
              setPlayerCount(Math.max(1, playerCount - 1));
            }}
            className="wood-button w-12 h-12 text-3xl font-black flex items-center justify-center pb-1"
          >
            -
          </motion.button>
          <div className="flex flex-col items-center w-24">
            <span className="text-3xl font-bangers text-[var(--color-game-dark-text)] drop-shadow-sm">{playerCount}</span>
            <span className="text-sm font-bold text-[var(--color-game-dark-text)] uppercase tracking-widest">Pemain</span>
          </div>
          <motion.button 
            whileTap={{ scale: 0.9 }}
            onClick={() => {
              playSfx('click');
              setPlayerCount(Math.min(4, playerCount + 1));
            }}
            className="wood-button w-12 h-12 text-3xl font-black flex items-center justify-center pb-1"
          >
            +
          </motion.button>
        </div>

        <div className="flex flex-col gap-3 w-full mb-10">
          <AnimatePresence>
            {Array.from({ length: playerCount }).map((_, i) => (
              <motion.input
                key={`player-input-${i}`}
                initial={{ opacity: 0, scaleY: 0, height: 0, marginTop: 0 }}
                animate={{ opacity: 1, scaleY: 1, height: 'auto', marginTop: i > 0 ? 12 : 0 }}
                exit={{ opacity: 0, scaleY: 0, height: 0, marginTop: 0 }}
                transition={{ type: "spring", bounce: 0.4 }}
                type="text"
                maxLength={14}
                placeholder={`Nama Pemain ${i + 1}...`}
                value={playerNames[i]}
                onChange={(e) => handleNameChange(i, e.target.value)}
                onKeyDown={stopProp}
                onKeyUp={stopProp}
                onKeyPress={stopProp}
                className="px-5 py-3 font-pixel font-bold text-lg rounded-lg border-[3px] border-[var(--color-game-dark-wood)] shadow-inner focus:outline-none focus:ring-4 focus:ring-[var(--color-game-wood)] transition-all placeholder:opacity-50 origin-top bg-[var(--color-game-parchment-light)] text-[var(--color-game-dark-text)]"
              />
            ))}
          </AnimatePresence>
        </div>

        <motion.button
          whileTap={{ scale: 0.95 }}
          onClick={handleStart}
          className="wood-button w-full py-4 text-xl sm:text-2xl font-black uppercase"
        >
          Mulai Bermain
        </motion.button>
      </motion.div>
    </div>
  );
};

export default MainMenu;
