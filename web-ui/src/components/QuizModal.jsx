import React, { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

export default function QuizModal({ quiz, onAnswer, onClose }) {
  const [selectedIdx, setSelectedIdx] = useState(null);
  const [hasAnswered, setHasAnswered] = useState(false);

  if (!quiz) return null;

  const handleOptionClick = (idx) => {
    if (hasAnswered) return;
    setSelectedIdx(idx);
    setHasAnswered(true);
    
    // Convert index to "A" or "B" for Unity C# mapping compatibility
    const answerChar = idx === 0 ? "A" : "B";
    onAnswer(answerChar);
  };

  const isCorrect = selectedIdx === quiz.correctAnswerIndex;

  return (
    <div className="absolute inset-0 bg-slate-950/80 backdrop-blur-sm z-50 flex items-center justify-center p-4 pointer-events-auto select-none font-playful">
      <motion.div
        className="w-full max-w-lg bg-game-dark/95 border-4 border-slate-700/60 rounded-cartoon p-6 text-center text-white"
        style={{
          boxShadow: hasAnswered 
            ? (isCorrect ? "0 0 35px rgba(21, 184, 90, 0.35), 0 8px 0 0 rgba(0, 0, 0, 0.45)" : "0 0 35px rgba(230, 62, 39, 0.35), 0 8px 0 0 rgba(0, 0, 0, 0.45)")
            : "0 8px 0 0 rgba(0, 0, 0, 0.45)",
          borderColor: hasAnswered
            ? (isCorrect ? "rgba(21, 184, 90, 0.6)" : "rgba(230, 62, 39, 0.6)")
            : "rgba(255, 255, 255, 0.15)"
        }}
        initial={{ scale: 0.8, opacity: 0 }}
        animate={{ scale: 1, opacity: 1 }}
        exit={{ scale: 0.8, opacity: 0 }}
        transition={{ type: "spring", stiffness: 300, damping: 20 }}
      >
        {/* Academic Header Badge */}
        <div className="inline-block bg-game-blue/20 text-game-cyan border border-game-blue/40 text-[10px] font-black uppercase px-3 py-1 rounded-full mb-3 tracking-widest">
          IPB Quiz Challenge 🎓
        </div>

        {/* Question text */}
        <h2 className="text-lg font-black leading-snug text-slate-100 mb-6 drop-shadow-[0_2px_4px_rgba(0,0,0,0.5)]">
          {quiz.questionText}
        </h2>

        {/* Question options */}
        {!hasAnswered ? (
          <div className="flex flex-col gap-3.5 mb-2">
            {quiz.choices.map((choice, idx) => {
              const labelPrefix = idx === 0 ? "A. " : "B. ";
              return (
                <button
                  key={idx}
                  onClick={() => handleOptionClick(idx)}
                  className="w-full text-left p-4 rounded-xl text-white font-extrabold text-sm uppercase transition-all duration-100 flex items-center gap-3 cursor-pointer bg-slate-900 border-2 border-slate-800 hover:bg-slate-850 hover:border-slate-700 active:translate-y-[2px] border-b-4 border-b-slate-950 active:border-b-2"
                >
                  <span className="flex items-center justify-center w-8 h-8 rounded-lg bg-slate-800 border border-slate-700 text-yellow-300 text-sm font-black">
                    {idx === 0 ? "A" : "B"}
                  </span>
                  <span className="flex-1 text-slate-200">{choice}</span>
                </button>
              );
            })}
          </div>
        ) : (
          /* Animated Bubbly Feedback panel */
          <motion.div
            initial={{ scale: 0.95, opacity: 0 }}
            animate={{ scale: 1, opacity: 1 }}
            className="flex flex-col items-center gap-4 bg-slate-900/60 p-5 rounded-2xl border border-slate-800 mb-2"
          >
            {/* Header statement */}
            <motion.h3
              className={`text-2xl font-black ${isCorrect ? "text-green-400" : "text-red-400"}`}
              animate={{ scale: [1, 1.1, 1] }}
              transition={{ duration: 0.3 }}
            >
              {isCorrect ? "BENAR! 🎉" : "KURANG TEPAT 😢"}
            </motion.h3>

            {/* Explanatory text */}
            <p className="text-xs font-semibold text-slate-300 leading-relaxed text-justify max-h-[140px] overflow-y-auto px-1.5">
              {isCorrect ? quiz.correctFeedback : quiz.incorrectFeedback}
            </p>

            {/* Bubbly dismiss button */}
            <button
              onClick={onClose}
              className={`mt-2 py-2.5 px-6 rounded-xl text-white font-black text-xs uppercase cursor-pointer transition-all duration-100 hover:scale-105 active:translate-y-1 border-b-4 ${
                isCorrect 
                  ? "bg-green-600 hover:bg-green-500 border-green-800 active:border-b-0" 
                  : "bg-red-600 hover:bg-red-500 border-red-800 active:border-b-0"
              }`}
            >
              Lanjutkan Perjalanan
            </button>
          </motion.div>
        )}
      </motion.div>
    </div>
  );
}
