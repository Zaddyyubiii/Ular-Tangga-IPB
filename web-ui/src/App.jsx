import React, { useState, useEffect } from 'react';
import PlayerCards from './components/PlayerCards';
import TopHUD from './components/TopHUD';
import RollDiceBar from './components/RollDiceBar';
import QuizModal from './components/QuizModal';
import PrologueModal from './components/PrologueModal';
import GameOverModal from './components/GameOverModal';
import { motion, AnimatePresence } from 'framer-motion';

function App() {
  const [gameState, setGameState] = useState(null);
  const [quiz, setQuiz] = useState(null);
  const [prologue, setPrologue] = useState(null);
  const [gameOver, setGameOver] = useState(null);

  // Listen to native Unity WebGL CustomEvents
  useEffect(() => {
    const handleStateUpdate = (e) => {
      console.log("React received state update:", e.detail);
      setGameState(e.detail);
    };

    const handleShowQuiz = (e) => {
      console.log("React received ShowQuiz:", e.detail);
      setQuiz(e.detail);
    };

    const handleShowPrologue = (e) => {
      console.log("React received ShowPrologue:", e.detail);
      setPrologue(e.detail);
    };

    const handleShowGameOver = (e) => {
      console.log("React received ShowGameOver:", e.detail);
      setGameOver(e.detail);
    };

    const handleCloseQuiz = () => {
      console.log("React received CloseQuiz");
      setQuiz(null);
    };

    const handleMainMenuLoaded = () => {
      console.log("React received MainMenuLoaded. Resetting all state.");
      setGameState(null);
      setQuiz(null);
      setPrologue(null);
      setGameOver(null);
    };

    window.addEventListener("UnityStateUpdated", handleStateUpdate);
    window.addEventListener("UnityShowQuiz", handleShowQuiz);
    window.addEventListener("UnityShowPrologue", handleShowPrologue);
    window.addEventListener("UnityShowGameOver", handleShowGameOver);
    window.addEventListener("UnityCloseQuiz", handleCloseQuiz);
    window.addEventListener("UnityMainMenuLoaded", handleMainMenuLoaded);

    return () => {
      window.removeEventListener("UnityStateUpdated", handleStateUpdate);
      window.removeEventListener("UnityShowQuiz", handleShowQuiz);
      window.removeEventListener("UnityShowPrologue", handleShowPrologue);
      window.removeEventListener("UnityShowGameOver", handleShowGameOver);
      window.removeEventListener("UnityCloseQuiz", handleCloseQuiz);
      window.removeEventListener("UnityMainMenuLoaded", handleMainMenuLoaded);
    };
  }, []);

  // Web bridge helper triggers
  const triggerUnityAction = (methodName, parameter) => {
    if (window.unityInstance) {
      console.log(`React calling C# Receiver: ${methodName}(${parameter})`);
      window.unityInstance.SendMessage("ReactReceiver", methodName, parameter);
    } else {
      console.warn("Unity instance not loaded yet or in mock testing environment.");
    }
  };

  return (
    <div className="relative w-screen h-screen overflow-hidden pointer-events-none select-none select-none flex flex-col items-center justify-between">
      {/* 1. Top HUD active capsule */}
      {gameState && !prologue && !gameOver && (
        <TopHUD 
          activePlayerName={gameState.players.find(p => p.id === gameState.activePlayerId)?.playerName || ""}
          activePlayerColor={gameState.players.find(p => p.id === gameState.activePlayerId)?.playerColorHex || "#ffffff"}
          timer={gameState.timerRemaining}
        />
      )}

      {/* 2. Player Corner Cards */}
      {gameState && !prologue && !gameOver && (
        <PlayerCards players={gameState.players} activePlayerId={gameState.activePlayerId} />
      )}

      {/* 3. Bottom Gauge & Control Panel */}
      {gameState && !prologue && !gameOver && (
        <RollDiceBar 
          players={gameState.players}
          activePlayerId={gameState.activePlayerId}
          instruction={gameState.instructionText}
          onRoll={(power) => triggerUnityAction("OnRollDice", power)}
        />
      )}

      {/* Floating Dice Result Banner */}
      <AnimatePresence>
        {gameState && gameState.showDiceResult && !prologue && !gameOver && (
          <motion.div
            initial={{ opacity: 0, scale: 0.8, y: 30 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.8, y: 20 }}
            transition={{ type: "spring", stiffness: 400, damping: 25 }}
            className="absolute bottom-36 left-1/2 -translate-x-1/2 z-20 flex flex-col items-center p-4 bg-slate-900/90 backdrop-blur-md rounded-cartoon border-4 border-slate-700/60 text-white min-w-[200px] text-center shadow-cartoon select-none pointer-events-auto"
            style={{
              boxShadow: "0 10px 0 0 rgba(0, 0, 0, 0.4)"
            }}
          >
            {/* Roller Title */}
            <span className="text-[10px] text-slate-400 font-extrabold uppercase tracking-widest mb-1">
              {gameState.diceValue === 0 ? "Melempar Dadu" : `Kocokan ${gameState.diceRollerName}`}
            </span>
            
            {/* Dice Value / Rolling Animation */}
            {gameState.diceValue === 0 ? (
              <div className="flex flex-col items-center gap-1.5 py-1.5">
                <motion.div 
                  className="text-4xl"
                  animate={{ rotate: 360 }}
                  transition={{ duration: 1, repeat: Infinity, ease: "linear" }}
                >
                  🎲
                </motion.div>
                <motion.span 
                  className="text-[11px] font-black text-slate-350 animate-pulse uppercase tracking-wider"
                >
                  {gameState.diceRollerName} sedang mengocok...
                </motion.span>
              </div>
            ) : (
              <>
                {/* Dice Value */}
                <div className="text-4xl font-black text-yellow-300 drop-shadow-[0_2px_4px_rgba(0,0,0,0.5)] my-1 animate-bounce">
                  🎲 {gameState.diceValue}
                </div>

                {/* Timing Quality */}
                {gameState.diceTimingQuality && (
                  <span className={`text-xs font-black uppercase tracking-wider ${
                    gameState.diceTimingQuality.includes("Perfect") ? "text-amber-400" :
                    gameState.diceTimingQuality.includes("Good") ? "text-emerald-400" : "text-slate-300"
                  }`}>
                    {gameState.diceTimingQuality}
                  </span>
                )}

                {/* Charge Percent */}
                <span className="text-[10px] font-bold text-slate-400 mt-1">
                  Power: {Math.round(gameState.diceChargePercent)}%
                </span>
              </>
            )}
          </motion.div>
        )}
      </AnimatePresence>

      {/* 4. Modal Overlays */}
      {prologue && (
        <PrologueModal 
          text={prologue.narrationText} 
          onStart={() => {
            setPrologue(null);
            triggerUnityAction("OnStartJourney", "");
          }}
        />
      )}

      {quiz && (
        <QuizModal 
          quiz={quiz} 
          onAnswer={(answer) => triggerUnityAction("OnAnswerQuiz", answer)}
          onClose={() => {
            setQuiz(null);
            triggerUnityAction("OnCloseQuizFeedback", "");
          }}
        />
      )}

      {gameOver && (
        <GameOverModal 
          winnerName={gameOver.winnerName}
          winnerColor={gameOver.winnerColorHex}
          message={gameOver.messageText}
          onPlayAgain={() => {
            setGameOver(null);
            triggerUnityAction("OnPlayAgain", "");
          }}
          onReturnMenu={() => {
            setGameOver(null);
            triggerUnityAction("OnReturnToMenu", "");
          }}
        />
      )}
    </div>
  );
}

export default App;
