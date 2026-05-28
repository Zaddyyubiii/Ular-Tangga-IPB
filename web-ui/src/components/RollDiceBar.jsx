import React, { useState, useEffect, useRef } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

export default function RollDiceBar({ players, activePlayerId, instruction, onRoll }) {
  const [charge, setCharge] = useState(0);
  const [isCharging, setIsCharging] = useState(false);
  const chargeRef = useRef(0);
  const directionRef = useRef(1); // 1 = up, -1 = down
  const animFrameId = useRef(null);
  const lastTimeRef = useRef(null);

  const activePlayer = players.find(p => p.id === activePlayerId);
  const isHumanTurn = activePlayer && !activePlayer.isBot;
  const isGameOverState = players.every(p => p.isFinished || p.isDroppedOut);

  // Constants matching C# settings
  const CHARGE_SPEED = 280; // Fast reflex-based!

  // Reset charge if turn changes
  useEffect(() => {
    stopCharging(false);
    setCharge(0);
  }, [activePlayerId]);

  // Handle global Space key listeners for charging
  useEffect(() => {
    if (!isHumanTurn || isGameOverState) return;

    const handleKeyDown = (e) => {
      if (e.code === 'Space' && !e.repeat && !isCharging) {
        e.preventDefault();
        startCharging();
      }
    };

    const handleKeyUp = (e) => {
      if (e.code === 'Space' && isCharging) {
        e.preventDefault();
        stopCharging(true);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    window.addEventListener('keyup', handleKeyUp);

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
      window.removeEventListener('keyup', handleKeyUp);
    };
  }, [isHumanTurn, isCharging, isGameOverState]);

  // Charging animation loop
  const chargeLoop = (timestamp) => {
    if (!lastTimeRef.current) lastTimeRef.current = timestamp;
    const delta = (timestamp - lastTimeRef.current) / 1000; // in seconds
    lastTimeRef.current = timestamp;

    let nextCharge = chargeRef.current + directionRef.current * CHARGE_SPEED * delta;

    if (nextCharge >= 100) {
      nextCharge = 100;
      directionRef.current = -1; // bounce down
    } else if (nextCharge <= 0) {
      nextCharge = 0;
      directionRef.current = 1; // bounce up
    }

    chargeRef.current = nextCharge;
    setCharge(nextCharge);

    animFrameId.current = requestAnimationFrame(chargeLoop);
  };

  const startCharging = () => {
    if (isCharging || !isHumanTurn) return;
    setIsCharging(true);
    chargeRef.current = 0;
    directionRef.current = 1;
    lastTimeRef.current = null;
    animFrameId.current = requestAnimationFrame(chargeLoop);
  };

  const stopCharging = (shouldRoll = true) => {
    if (animFrameId.current) {
      cancelAnimationFrame(animFrameId.current);
      animFrameId.current = null;
    }
    setIsCharging(false);
    
    if (shouldRoll && chargeRef.current > 0) {
      onRoll(chargeRef.current);
    }
  };

  // Determine current active zone and dice range
  const getZoneInfo = (val) => {
    if (val <= 20) return { label: "Zona 1 (Status Percobaan)", range: "1 - 3", color: "from-cyan-400 to-cyan-500" };
    if (val <= 40) return { label: "Zona 2 (Mahasiswa Reguler)", range: "3 - 5", color: "from-blue-400 to-blue-500" };
    if (val <= 60) return { label: "Zona 3 (Mahasiswa Reguler)", range: "5 - 8", color: "from-green-400 to-green-500" };
    if (val <= 80) return { label: "Zona 4 (Mahasiswa Teladan)", range: "8 - 10", color: "from-orange-400 to-orange-500" };
    return { label: "Zona 5 (Duta Tata Tertib)", range: "10 - 12", color: "from-yellow-400 to-yellow-500 animate-pulse" };
  };

  const zone = getZoneInfo(charge);

  // Calculate needed tiles
  let neededSuffix = "";
  if (activePlayer) {
    const needed = 100 - activePlayer.currentTile;
    if (needed > 0 && needed < 12) {
      neededSuffix = ` (Butuh ${needed})`;
    }
  }

  return (
    <div className="absolute bottom-4 left-0 right-0 flex flex-col items-center gap-2 z-10 w-full px-4 select-none">
      {/* Instruction bubble */}
      <AnimatePresence mode="wait">
        {instruction && (
          <motion.div
            key={instruction}
            className="px-4 py-1.5 rounded-full bg-slate-900/90 backdrop-blur-sm border border-slate-700 text-[11px] font-bold text-slate-200 tracking-wide text-center"
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: -10 }}
            transition={{ duration: 0.15 }}
          >
            {instruction}
          </motion.div>
        )}
      </AnimatePresence>

      {/* Main Flat Charging Panel */}
      <div 
        className={`w-full max-w-[580px] p-2 bg-game-dark/85 backdrop-blur-md rounded-cartoon border-4 border-white/10 shadow-bubble flex items-center justify-between gap-3 ${
          !isHumanTurn ? "opacity-70 pointer-events-none" : "pointer-events-auto"
        }`}
        style={{
          boxShadow: isCharging 
            ? `0 0 20px rgba(0, 204, 255, 0.2), 0 6px 0 0 rgba(0, 0, 0, 0.3)` 
            : "0 6px 0 0 rgba(0, 0, 0, 0.3)"
        }}
      >
        {/* Left Side: Stats and Info labels */}
        <div className="flex flex-col w-40 text-left pl-2">
          <span className="text-[10px] text-slate-400 font-extrabold uppercase truncate">
            {isCharging ? zone.label : "Siap Mengocok"}
          </span>
          <span className="text-xs font-black text-yellow-300 truncate">
            Dadu: {zone.range}{neededSuffix}
          </span>
        </div>

        {/* Center: Slider container */}
        <div className="flex-1 h-8 bg-slate-950/80 rounded-xl border-2 border-slate-800 relative overflow-hidden flex items-center px-1">
          {/* Slider gauge fill */}
          <motion.div 
            className={`h-5 rounded-lg bg-gradient-to-r ${zone.color}`}
            style={{ width: `${Math.max(2, charge)}%` }}
            transition={{ type: "tween", ease: "linear" }}
          />

          {/* Grid markers for standard zones */}
          <div className="absolute top-0 bottom-0 left-[20%] w-0.5 bg-slate-800/40" />
          <div className="absolute top-0 bottom-0 left-[40%] w-0.5 bg-slate-800/40" />
          <div className="absolute top-0 bottom-0 left-[60%] w-0.5 bg-slate-800/40" />
          <div className="absolute top-0 bottom-0 left-[80%] w-0.5 bg-slate-800/40" />

          {/* Centered power percent text */}
          <span className="absolute left-1/2 -translate-x-1/2 text-xs font-black text-white drop-shadow-[0_1.5px_1.5px_rgba(0,0,0,0.8)]">
            POWER: {Math.round(charge)}%
          </span>
        </div>

        {/* Right Side: Roll bubble button */}
        <button
          className={`h-11 px-5 rounded-xl text-white font-black text-sm uppercase transition-all duration-100 flex items-center justify-center cursor-pointer select-none active:translate-y-[4px] border-b-4 ${
            isHumanTurn 
              ? "bg-game-orange hover:bg-game-orange/90 active:border-b-0 border-orange-800 active:translate-y-1" 
              : "bg-slate-700 border-slate-800 text-slate-400 cursor-not-allowed"
          }`}
          style={{
            boxShadow: isHumanTurn && !isCharging ? "0 4px 0 0 rgba(154, 43, 22, 1)" : "none"
          }}
          onMouseDown={startCharging}
          onMouseUp={() => stopCharging(true)}
          onTouchStart={(e) => { e.preventDefault(); startCharging(); }}
          onTouchEnd={(e) => { e.preventDefault(); stopCharging(true); }}
          disabled={!isHumanTurn}
        >
          {isCharging ? "Kocok!" : "ROLL"}
        </button>
      </div>
    </div>
  );
}
