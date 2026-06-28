import { useState, useEffect, useRef } from 'react';
import PlayerCards from './components/PlayerCards';
import TopHUD from './components/TopHUD';
import Dice3D from './components/Dice3D';
import BoardDiceThrow from './components/BoardDiceThrow';
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
  const [boardDiceValue, setBoardDiceValue] = useState(null);
  const [isLocalCharging, setIsLocalCharging] = useState(false); // React-side charging state
  const prevShowDiceRef = useRef(false);
  const prevDiceValueRef = useRef(0);

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

  // Detect dice result arrival — triggers board throw animation.
  // Works for both human (no diceValue=0 phase) and bot (has diceValue=0 phase).
  // Fires when showDiceResult flips to true AND diceValue > 0.
  useEffect(() => {
    const showNow = gameState?.showDiceResult ?? false;
    const valNow = gameState?.diceValue ?? 0;
    const showPrev = prevShowDiceRef.current;
    const valPrev = prevDiceValueRef.current;

    if (valNow > 0 && (
      // Case 1: showDiceResult just became true with a value (human player path)
      (showNow && !showPrev) ||
      // Case 2: was showing rolling (val=0) and result just arrived (bot path)
      (showNow && valPrev === 0 && valNow > 0)
    )) {
      setIsLocalCharging(false); // kill rolling banner
      setBoardDiceValue(valNow);
    }

    // Clear board dice when showDiceResult turns off
    if (!showNow && showPrev) {
      setBoardDiceValue(null);
    }

    prevShowDiceRef.current = showNow;
    prevDiceValueRef.current = valNow;
  }, [gameState?.showDiceResult, gameState?.diceValue]);

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

  // Rolling banner shows when:
  // - Human player is charging (React local state), OR
  // - Bot is rolling (Unity sends diceValue === 0)
  const isUnityRolling = gameState && gameState.showDiceResult && gameState.diceValue === 0;
  const showRollingBanner = (isLocalCharging || isUnityRolling) && !isPopupOpen;
  // Who is rolling — use diceRollerName if available, else active player
  const rollerName = gameState?.diceRollerName || activePlayerName;
  const rollerColor = gameState?.players?.find(p => p.playerName === rollerName)?.playerColorHex || "rgba(100, 116, 139, 0.6)";

  // Score banner: ONLY after result arrives (diceValue > 0), separate from rolling
  const showScoreBanner = gameState && gameState.showDiceResult && gameState.diceValue > 0 && !isPopupOpen;

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
          onRoll={(power) => {
            triggerUnityAction("OnRollDice", power);
            // isLocalCharging will be cleared by the dice result effect above
          }}
          onChargingChange={setIsLocalCharging}
          gameState={gameState}
          isPopupOpen={isPopupOpen}
        />
      )}

      {/* 3D Dice Throw onto Board — triggers the moment diceValue goes from 0→N */}
      <BoardDiceThrow
        value={boardDiceValue}
        visible={boardDiceValue != null && boardDiceValue >= 2}
        onDone={() => setBoardDiceValue(null)}
      />

      {/* Rolling Banner — shows ONLY while dice are shaking (diceValue === 0) */}
      <AnimatePresence>
        {showRollingBanner && (
          <motion.div
            key="rolling-banner"
            initial={{ opacity: 0, scale: 0.8, y: 30 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.6, y: -20 }}
            transition={{ type: "spring", stiffness: 400, damping: 25, exit: { duration: 0.15 } }}
            className="absolute bottom-36 left-1/2 -translate-x-1/2 z-20 flex flex-col items-center p-4 bg-slate-900/90 backdrop-blur-md rounded-cartoon border-4 text-white min-w-[200px] text-center shadow-cartoon select-none pointer-events-auto"
            style={{
              boxShadow: "0 10px 0 0 rgba(0, 0, 0, 0.4)",
              borderColor: rollerColor
            }}
          >
            <span className="text-[10px] text-slate-400 font-extrabold uppercase tracking-widest mb-1">
              Melempar Dadu
            </span>
            <div className="flex flex-col items-center gap-1.5 py-1.5">
              <Dice3D value={0} isRolling={true} />
              <motion.span
                className="text-[11px] font-black text-slate-350 animate-pulse uppercase tracking-wider mt-4"
              >
                {rollerName} sedang mengocok...
              </motion.span>
            </div>
          </motion.div>
        )}
      </AnimatePresence>

      {/* Score Banner — appears after board dice land, shows result + quality */}
      <AnimatePresence>
        {showScoreBanner && (
          <motion.div
            key="score-banner"
            initial={{ opacity: 0, scale: 0.5, y: 40 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            exit={{ opacity: 0, scale: 0.8, y: 20 }}
            transition={{ type: "spring", stiffness: 300, damping: 22, delay: 1.0 }}
            className="absolute bottom-36 left-1/2 -translate-x-1/2 z-20 flex flex-col items-center p-4 bg-slate-900/90 backdrop-blur-md rounded-cartoon border-4 text-white min-w-[200px] text-center shadow-cartoon select-none pointer-events-auto"
            style={{
              boxShadow: "0 10px 0 0 rgba(0, 0, 0, 0.4)",
              borderColor: gameState.players.find(p => p.playerName === gameState.diceRollerName)?.playerColorHex || "rgba(100, 116, 139, 0.6)"
            }}
          >
            <span className="text-[10px] text-slate-400 font-extrabold uppercase tracking-widest mb-1">
              Kocokan {gameState.diceRollerName}
            </span>

            {/* Big Score Number */}
            <motion.div
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{ type: "spring", stiffness: 500, damping: 15 }}
              className="text-5xl font-black text-yellow-300 drop-shadow-[0_2px_4px_rgba(0,0,0,0.5)] my-2"
            >
              {gameState.diceValue}
            </motion.div>

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
