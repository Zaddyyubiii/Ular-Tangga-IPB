import { useState, useEffect } from 'react';
import PlayerCards from './components/PlayerCards';
import TopHUD from './components/TopHUD';
import RollDiceBar from './components/RollDiceBar';
import QuizModal from './components/QuizModal';
import PrologueModal from './components/PrologueModal';
import GameOverModal from './components/GameOverModal';
import MainMenu from './components/MainMenu';
import PopupModal from './components/PopupModal';
import { motion, AnimatePresence } from 'framer-motion';

function App() {
  const [gameState, setGameState] = useState(null);
  const [quiz, setQuiz] = useState(null);
  const [prologue, setPrologue] = useState(null);
  const [gameOver, setGameOver] = useState(null);
  const [isMainMenu, setIsMainMenu] = useState(true);
  const [popupData, setPopupData] = useState(null);

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

    const handleShowPopup = (e) => {
      console.log("React received ShowPopup:", e.detail);
      setPopupData(e.detail);
    };

    const handleClosePopup = () => {
      console.log("React received ClosePopup");
      setPopupData(null);
    };

    const handleMainMenuLoaded = () => {
      console.log("React received MainMenuLoaded. Resetting all state.");
      setGameState(null);
      setQuiz(null);
      setPrologue(null);
      setGameOver(null);
      setPopupData(null);
      setIsMainMenu(true);
    };

    window.addEventListener("UnityStateUpdated", handleStateUpdate);
    window.addEventListener("UnityShowQuiz", handleShowQuiz);
    window.addEventListener("UnityShowPrologue", handleShowPrologue);
    window.addEventListener("UnityShowGameOver", handleShowGameOver);
    window.addEventListener("UnityCloseQuiz", handleCloseQuiz);
    window.addEventListener("UnityShowPopup", handleShowPopup);
    window.addEventListener("UnityClosePopup", handleClosePopup);
    window.addEventListener("UnityMainMenuLoaded", handleMainMenuLoaded);

    return () => {
      window.removeEventListener("UnityStateUpdated", handleStateUpdate);
      window.removeEventListener("UnityShowQuiz", handleShowQuiz);
      window.removeEventListener("UnityShowPrologue", handleShowPrologue);
      window.removeEventListener("UnityShowGameOver", handleShowGameOver);
      window.removeEventListener("UnityCloseQuiz", handleCloseQuiz);
      window.removeEventListener("UnityShowPopup", handleShowPopup);
      window.removeEventListener("UnityClosePopup", handleClosePopup);
      window.removeEventListener("UnityMainMenuLoaded", handleMainMenuLoaded);
    };
  }, []);

  const triggerUnityAction = (methodName, parameter) => {
    if (window.unityInstance) {
      console.log(`React calling C# Receiver: ${methodName}(${parameter})`);
      window.unityInstance.SendMessage("ReactReceiver", methodName, parameter);
    } else {
      console.warn("Unity instance not loaded yet or in mock testing environment.");
    }
  };

  const activePlayer = gameState?.players?.find(p => p.id === gameState.activePlayerId);
  const activePlayerName = activePlayer?.playerName || "";
  const isPopupOpen = !!(popupData || quiz || prologue || gameOver);
  const showDiceBanner = gameState && gameState.showDiceResult && !isPopupOpen && gameState.diceRollerName === activePlayerName;

  return (
    <div className="relative w-screen h-screen overflow-hidden pointer-events-none flex flex-col items-center justify-between">
      {/* 0. Main Menu */}
      <AnimatePresence>
        {isMainMenu && (
          <MainMenu 
            onStartGame={(data) => {
              setIsMainMenu(false);
              triggerUnityAction("OnStartGameFromReact", JSON.stringify(data));
            }} 
          />
        )}
      </AnimatePresence>

      {/* 1. Top HUD active capsule */}
      {gameState && !prologue && !gameOver && !isMainMenu && (
        <TopHUD 
          activePlayerName={activePlayerName}
          activePlayerColor={activePlayer?.playerColorHex || "#ffffff"}
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
          gameState={gameState}
          isPopupOpen={isPopupOpen}
        />
      )}

      {/* Floating Dice Result Banner */}
      <AnimatePresence>
        {showDiceBanner && (
          <motion.div
            initial={{ opacity: 0, scale: 0.8, y: 30 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.8, y: 20 }}
            transition={{ type: "spring", stiffness: 400, damping: 25 }}
            className="absolute bottom-36 left-1/2 -translate-x-1/2 z-20 flex flex-col items-center p-4 bg-slate-900/90 backdrop-blur-md rounded-cartoon border-4 text-white min-w-[200px] text-center shadow-cartoon select-none pointer-events-auto"
            style={{
              boxShadow: "0 10px 0 0 rgba(0, 0, 0, 0.4)",
              borderColor: gameState.players.find(p => p.playerName === gameState.diceRollerName)?.playerColorHex || "rgba(100, 116, 139, 0.6)"
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

      {/* 4. Quiz Modal overlay */}
      <QuizModal 
        key={quiz ? quiz.questionText : "empty-quiz"}
        quiz={quiz} 
        onAnswer={(answer) => triggerUnityAction("OnAnswerQuiz", answer)}
        onClose={() => {
          setQuiz(null);
          triggerUnityAction("OnCloseQuizFeedback", "");
        }}
      />

      {/* 4b. Normal Popup Modal overlay */}
      <PopupModal popupData={popupData} triggerUnityAction={triggerUnityAction} />

      {/* 5. Prologue overlay */}
      {prologue && (
        <PrologueModal 
          text={prologue.narrationText} 
          onStart={() => {
            setPrologue(null);
            triggerUnityAction("OnStartJourney", "");
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
