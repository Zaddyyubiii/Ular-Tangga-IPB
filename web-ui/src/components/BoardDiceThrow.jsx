import React, { useEffect, useState, useRef } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import './Dice3D.css';

/**
 * BoardDiceThrow — Renders 2 physical 3D dice thrown onto the game board.
 *
 * Triggered when diceValue goes from 0 (rolling) to a real value (2-12).
 * The dice fly in from above, bounce, tumble, and land showing correct faces.
 * Renders over the Unity canvas but doesn't block pointer events.
 *
 * Props:
 *   value    — total dice result (2-12) from Unity
 *   visible  — whether to show the throw animation
 *   onDone   — callback when animation finishes (optional)
 */

function splitIntoDice(total) {
  const clamped = Math.max(2, Math.min(12, total || 2));
  const pairs = [];
  for (let d1 = 1; d1 <= 6; d1++) {
    const d2 = clamped - d1;
    if (d2 >= 1 && d2 <= 6) pairs.push([d1, d2]);
  }
  return pairs[Math.floor(Math.random() * pairs.length)];
}

// Rotation needed to show each face (must match Dice3D.css .show-N)
const FACE_ROTATIONS = {
  1: { rotateX: 0,    rotateY: 0 },
  2: { rotateX: 0,    rotateY: -90 },
  3: { rotateX: -90,  rotateY: 0 },
  4: { rotateX: 90,   rotateY: 0 },
  5: { rotateX: 0,    rotateY: 90 },
  6: { rotateX: 180,  rotateY: 0 },
};

function ThrownDie({ face, index, onComplete }) {
  const finalRot = FACE_ROTATIONS[face] || FACE_ROTATIONS[1];

  // Each die gets slightly different trajectory for natural feel
  const isLeft = index === 0;
  // Landing positions: spread apart horizontally, centered vertically on board
  const landX = isLeft ? -45 : 45;
  const landY = 0;

  // Random extra spins before landing (multiples of 360 + final rotation)
  const extraSpinsX = (2 + Math.floor(Math.random() * 2)) * 360;
  const extraSpinsY = (1 + Math.floor(Math.random() * 2)) * 360;
  const extraSpinsZ = (1 + Math.floor(Math.random() * 2)) * 360;

  return (
    <motion.div
      style={{
        position: 'absolute',
        left: '50%',
        top: '50%',
        marginLeft: '-35px',
        marginTop: '-35px',
        perspective: '800px',
        transformStyle: 'preserve-3d',
        filter: 'drop-shadow(0 20px 30px rgba(0,0,0,0.6))',
        zIndex: 50,
      }}
      initial={{
        x: isLeft ? -200 : 200,
        y: -500,
        scale: 0.3,
        opacity: 0,
      }}
      animate={{
        x: [
          isLeft ? -200 : 200,   // start off-center
          isLeft ? -80 : 80,     // mid-air
          landX + (isLeft ? 15 : -15), // first bounce overshoot
          landX,                 // settle
        ],
        y: [
          -500,    // start above screen
          -50,     // approach board
          -30,     // first bounce up
          landY,   // settle
        ],
        scale: [0.3, 1.1, 0.95, 1],
        opacity: [0, 1, 1, 1],
      }}
      transition={{
        duration: 1.0,
        times: [0, 0.5, 0.75, 1],
        ease: [0.25, 0.1, 0.25, 1],
        delay: index * 0.12,
      }}
      onAnimationComplete={() => {
        if (onComplete) onComplete();
      }}
    >
      <motion.div
        style={{
          width: '70px',
          height: '70px',
          transformStyle: 'preserve-3d',
        }}
        initial={{
          rotateX: 0,
          rotateY: 0,
          rotateZ: 0,
        }}
        animate={{
          rotateX: [0, extraSpinsX + finalRot.rotateX],
          rotateY: [0, extraSpinsY + finalRot.rotateY],
          rotateZ: [0, extraSpinsZ, extraSpinsZ * 0.2, 0],
        }}
        transition={{
          duration: 1.0,
          times: [0, 1],
          ease: 'easeOut',
          delay: index * 0.12,
          rotateZ: {
            duration: 1.0,
            times: [0, 0.5, 0.8, 1],
            ease: 'easeOut',
          },
        }}
      >
        {/* Face 1 */}
        <div className="dice-face face-1">
          <div className="dot dot-center" />
        </div>
        {/* Face 2 */}
        <div className="dice-face face-2">
          <div className="dot dot-top-right" />
          <div className="dot dot-bottom-left" />
        </div>
        {/* Face 3 */}
        <div className="dice-face face-3">
          <div className="dot dot-top-right" />
          <div className="dot dot-center" />
          <div className="dot dot-bottom-left" />
        </div>
        {/* Face 4 */}
        <div className="dice-face face-4">
          <div className="dot dot-top-left" />
          <div className="dot dot-top-right" />
          <div className="dot dot-bottom-left" />
          <div className="dot dot-bottom-right" />
        </div>
        {/* Face 5 */}
        <div className="dice-face face-5">
          <div className="dot dot-top-left" />
          <div className="dot dot-top-right" />
          <div className="dot dot-center" />
          <div className="dot dot-bottom-left" />
          <div className="dot dot-bottom-right" />
        </div>
        {/* Face 6 */}
        <div className="dice-face face-6">
          <div className="dot dot-top-left" />
          <div className="dot dot-top-right" />
          <div className="dot dot-mid-left" />
          <div className="dot dot-mid-right" />
          <div className="dot dot-bottom-left" />
          <div className="dot dot-bottom-right" />
        </div>
      </motion.div>
    </motion.div>
  );
}

export default function BoardDiceThrow({ value, visible, onDone }) {
  const [faces, setFaces] = useState([1, 1]);
  const [show, setShow] = useState(false);
  const doneCount = useRef(0);

  useEffect(() => {
    if (visible && value && value >= 2) {
      setFaces(splitIntoDice(value));
      setShow(true);
      doneCount.current = 0;
    } else {
      setShow(false);
    }
  }, [visible, value]);

  const handleDieComplete = () => {
    doneCount.current += 1;
    if (doneCount.current >= 2) {
      // Keep dice visible for a beat after landing, then fade
      setTimeout(() => {
        setShow(false);
        if (onDone) onDone();
      }, 1800);
    }
  };

  return (
    <AnimatePresence>
      {show && (
        <motion.div
          key="board-dice-throw"
          className="absolute inset-0 pointer-events-none"
          style={{ zIndex: 15 }}
          initial={{ opacity: 1 }}
          exit={{ opacity: 0 }}
          transition={{ duration: 0.4 }}
        >
          <ThrownDie face={faces[0]} index={0} onComplete={handleDieComplete} />
          <ThrownDie face={faces[1]} index={1} onComplete={handleDieComplete} />
        </motion.div>
      )}
    </AnimatePresence>
  );
}
