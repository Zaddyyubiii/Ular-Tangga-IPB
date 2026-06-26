import React, { useEffect, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const PopupModal = ({ popupData, triggerUnityAction }) => {
  const [isVisible, setIsVisible] = useState(false);
  
  useEffect(() => {
    if (popupData) {
      setIsVisible(true);
    } else {
      setIsVisible(false);
    }
  }, [popupData]);

  const handleContinue = () => {
    setIsVisible(false);
    triggerUnityAction("OnPopupClosedFromReact", "");
  };

  if (!popupData) return null;

  // Determine colors based on title (similar to Unity's logic)
  const lowerTitle = (popupData.title || "").toLowerCase();
  let borderColor = "var(--color-game-dark-wood)";
  let titleBg = "var(--color-game-wood)";
  let titleColor = "var(--color-game-cream-text)";
  
  if (lowerTitle.includes("prestasi") || lowerTitle.includes("duta") || lowerTitle.includes("selamat") || lowerTitle.includes("kegiatan positif")) {
    borderColor = "var(--color-game-deep-grass)";
    titleBg = "var(--color-game-grass)";
    titleColor = "var(--color-game-dark-text)";
  } else if (lowerTitle.includes("pelanggaran berat") || lowerTitle.includes("skors") || lowerTitle.includes("sanksi")) {
    borderColor = "#8b0000"; // Dark red
    titleBg = "#E84C4C"; // Red
    titleColor = "white";
  } else if (lowerTitle.includes("pelanggaran") || lowerTitle.includes("ular")) {
    borderColor = "#b45f06"; // Dark orange
    titleBg = "#f1c232"; // Yellow
    titleColor = "var(--color-game-dark-text)";
  } else {
    borderColor = "var(--color-game-dark-wood)";
    titleBg = "var(--color-game-wood)";
    titleColor = "var(--color-game-cream-text)";
  }

  return (
    <div className="absolute inset-0 z-50 flex items-center justify-center bg-slate-900/40 backdrop-blur-sm pointer-events-auto">
      <AnimatePresence>
        {isVisible && (
          <motion.div
            initial={{ scale: 0, rotate: -5 }}
            animate={{ scale: 1, rotate: 0 }}
            exit={{ scale: 0, opacity: 0 }}
            transition={{ type: "spring", damping: 15, stiffness: 200 }}
            className="w-[600px] max-w-[90vw] parchment-panel shadow-2xl flex flex-col overflow-hidden relative"
            style={{ borderColor: borderColor }}
          >
            {/* Title Bar */}
            <div className="w-full py-4 px-6 flex items-center justify-center border-b-4 border-black/20" style={{ backgroundColor: titleBg }}>
              <h2 className="text-3xl font-bangers drop-shadow-md text-center uppercase" style={{ color: titleColor }}>
                {popupData.title}
              </h2>
            </div>

            {/* Message Body */}
            <div className="p-8 flex flex-col items-center text-center">
              <p className="text-xl font-black text-[var(--color-game-dark-text)] mb-8 leading-relaxed font-playful whitespace-pre-wrap">
                {popupData.message}
              </p>

              {popupData.showContinueButton && (
                <motion.button
                  whileTap={{ scale: 0.95 }}
                  onClick={handleContinue}
                  className="wood-button px-10 py-3 text-2xl font-bangers"
                >
                  Lanjut
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 inline-block ml-2 -mt-1" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M12.293 5.293a1 1 0 011.414 0l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-2.293-2.293a1 1 0 010-1.414z" clipRule="evenodd" />
                  </svg>
                </motion.button>
              )}
              
              {!popupData.showContinueButton && (
                <div className="mt-4 flex items-center gap-3 opacity-60">
                  <div className="w-5 h-5 rounded-full border-2 border-amber-900 border-t-transparent animate-spin"></div>
                  <span className="text-amber-900 font-bold text-sm">Menunggu aksi bot...</span>
                </div>
              )}
            </div>
            
            {/* Explosion flash effect */}
            {popupData.playExplosion && (
              <motion.div 
                initial={{ opacity: 1 }}
                animate={{ opacity: 0 }}
                transition={{ duration: 0.5, delay: 0.1 }}
                className="absolute inset-0 bg-rose-500 z-50 pointer-events-none mix-blend-overlay"
              />
            )}
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default PopupModal;
