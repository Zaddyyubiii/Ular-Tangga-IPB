import React, { useState, useEffect } from 'react';
import PlayerCards from './components/PlayerCards';
import TopHUD from './components/TopHUD';
import RollDiceBar from './components/RollDiceBar';
import QuizModal from './components/QuizModal';
import PrologueModal from './components/PrologueModal';
import GameOverModal from './components/GameOverModal';

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

    window.addEventListener("UnityStateUpdated", handleStateUpdate);
    window.addEventListener("UnityShowQuiz", handleShowQuiz);
    window.addEventListener("UnityShowPrologue", handleShowPrologue);
    window.addEventListener("UnityShowGameOver", handleShowGameOver);

    return () => {
      window.removeEventListener("UnityStateUpdated", handleStateUpdate);
      window.removeEventListener("UnityShowQuiz", handleShowQuiz);
      window.removeEventListener("UnityShowPrologue", handleShowPrologue);
      window.removeEventListener("UnityShowGameOver", handleShowGameOver);
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
