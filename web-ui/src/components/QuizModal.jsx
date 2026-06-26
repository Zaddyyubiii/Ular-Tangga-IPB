import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

export default function QuizModal({ quiz, onAnswer, onClose }) {
  const [selectedIdx, setSelectedIdx] = useState(null);
  const [hasAnswered, setHasAnswered] = useState(false);

  // Synchronize Bot Answers automatically
  useEffect(() => {
    const handleBotAnswer = (e) => {
      const idx = e.detail.selectedIndex;
      console.log("[React QuizModal] Bot answer received:", idx);
      setSelectedIdx(idx);
      setHasAnswered(true);
    };
    window.addEventListener("UnityQuizAnswered", handleBotAnswer);
    return () => {
      window.removeEventListener("UnityQuizAnswered", handleBotAnswer);
    };
  }, []);

  if (!quiz) return null;

  const handleOptionClick = (idx) => {
    if (hasAnswered) return;
    setSelectedIdx(idx);
    setHasAnswered(true);
    
    // Convert index to letters "A", "B", "C", "D" for Unity C# mapping compatibility
    const answerChar = String.fromCharCode(65 + idx);
    onAnswer(answerChar);
  };

  const isCorrect = selectedIdx === quiz.correctAnswerIndex;

  return (
    <div className="absolute inset-0 bg-slate-900/40 backdrop-blur-sm z-50 flex items-center justify-center p-4 pointer-events-auto select-none font-playful">
      <motion.div
        className="w-full max-w-lg parchment-panel shadow-2xl p-6 text-center"
        style={{
          borderColor: hasAnswered
            ? (isCorrect ? "var(--color-game-deep-grass)" : "#8b0000")
            : "var(--color-game-dark-wood)",
          backgroundColor: "var(--color-game-parchment-light)"
        }}
        initial={{ scale: 0.8, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        exit={{ scale: 0.8, opacity: 0 }}
        transition={{ type: "spring", stiffness: 300, damping: 20 }}
      >
        {/* Academic Header Badge */}
        <div className="inline-block bg-[var(--color-game-sky)] text-[var(--color-game-dark-text)] border-2 border-black/20 text-[10px] font-black uppercase px-3 py-1 rounded-md mb-3 tracking-widest font-bangers">
          Kuis Tata Tertib 🎓
        </div>

        {/* Question text */}
        <h2 className="text-xl font-black leading-snug text-[var(--color-game-dark-text)] mb-6 drop-shadow-sm font-playful">
          {quiz.questionText}
        </h2>

        {/* Question options */}
        {!hasAnswered ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3.5 mb-2">
            {quiz.choices.map((choice, idx) => {
              const labelLetter = String.fromCharCode(65 + idx);
              return (
                <button
                  key={idx}
                  onClick={() => handleOptionClick(idx)}
                  className="wood-button w-full text-left p-4 text-[var(--color-game-cream-text)] font-extrabold text-sm transition-all duration-100 flex items-center gap-3 border-[var(--color-game-dark-wood)] hover:brightness-110"
                  style={{ textTransform: 'none' }}
                >
                  <span className="flex items-center justify-center w-8 h-8 rounded-lg bg-[var(--color-game-dark-wood)] text-yellow-300 text-lg font-bangers">
                    {labelLetter}
                  </span>
                  <span className="flex-1 font-playful">{choice}</span>
                </button>
              );
            })}
          </div>
        ) : (
          /* Animated Bubbly Feedback panel */
          <motion.div
            initial={{ scale: 0.95, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            className="flex flex-col items-center gap-4 bg-[var(--color-game-parchment)] p-5 rounded-2xl border-4 border-black/10 mb-2"
          >
            {/* Header statement */}
            <motion.h3
              className={`text-3xl font-bangers ${isCorrect ? "text-[var(--color-game-deep-grass)]" : "text-[#8b0000]"}`}
              animate={{ scale: [1, 1.1, 1] }}
              transition={{ duration: 0.3 }}
            >
              {isCorrect ? "BENAR! 🎉" : "KURANG TEPAT 😢"}
            </motion.h3>

            {/* Explanatory text */}
            <p className="text-sm font-bold text-[var(--color-game-dark-text)] leading-relaxed text-center max-h-[140px] overflow-y-auto px-1.5 font-playful">
              {isCorrect ? quiz.correctFeedback : quiz.incorrectFeedback}
            </p>

            {/* Bubbly dismiss button */}
            <button
              onClick={onClose}
              className={`wood-button mt-2 py-3 px-8 text-xl font-bangers uppercase transition-all duration-100 ${
                isCorrect 
                  ? "bg-[var(--color-game-grass)] border-[var(--color-game-deep-grass)]" 
                  : "bg-[#E84C4C] border-[#8b0000]"
              }`}
              style={{ backgroundColor: isCorrect ? 'var(--color-game-grass)' : '#E84C4C', borderColor: isCorrect ? 'var(--color-game-deep-grass)' : '#8b0000' }}
            >
              Lanjutkan Perjalanan
            </button>
          </motion.div>
        )}
      </motion.div>
    </div>
  );
}
