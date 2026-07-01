import { useEffect, useState } from 'react';
import './Dice3D.css';

/**
 * Split a total dice value (2-12) into two valid dice faces (1-6 each).
 * Uses random-ish split so the same total can show different combos.
 * Edge case: if Unity sends value < 2 (e.g. 1 from near-finish logic),
 * we clamp to minimum 2 since 2 physical dice can't roll less than 2.
 */
function splitDiceValue(total) {
  const clamped = Math.max(2, Math.min(12, total || 2));

  // Find all valid (d1, d2) pairs where d1+d2 = clamped, 1<=d1<=6, 1<=d2<=6
  const pairs = [];
  for (let d1 = 1; d1 <= 6; d1++) {
    const d2 = clamped - d1;
    if (d2 >= 1 && d2 <= 6) {
      pairs.push([d1, d2]);
    }
  }

  // Pick a random valid pair for visual variety
  return pairs[Math.floor(Math.random() * pairs.length)];
}

/**
 * Renders a single 3D CSS die showing a specific face (1-6).
 *
 * Standard die layout (opposite faces sum to 7):
 *   Front  = 1,  Back   = 6  (rotateY 0° / 180°)
 *   Right  = 2,  Left   = 5  (rotateY 90° / -90°)
 *   Top    = 3,  Bottom = 4  (rotateX 90° / -90°)
 */
const SingleDie = ({ face, isRolling, delayOffset = 0 }) => (
  <div className="dice-container">
    <div
      className={`dice-3d ${isRolling ? 'rolling' : `show-${face}`}`}
      style={isRolling ? { animationDelay: `${delayOffset}ms` } : undefined}
    >
      {/* Face 1 — single center dot (red) */}
      <div className="dice-face face-1">
        <div className="dot dot-center" />
      </div>

      {/* Face 2 — diagonal: top-right, bottom-left */}
      <div className="dice-face face-2">
        <div className="dot dot-top-right" />
        <div className="dot dot-bottom-left" />
      </div>

      {/* Face 3 — diagonal: top-right, center, bottom-left */}
      <div className="dice-face face-3">
        <div className="dot dot-top-right" />
        <div className="dot dot-center" />
        <div className="dot dot-bottom-left" />
      </div>

      {/* Face 4 — four corners */}
      <div className="dice-face face-4">
        <div className="dot dot-top-left" />
        <div className="dot dot-top-right" />
        <div className="dot dot-bottom-left" />
        <div className="dot dot-bottom-right" />
      </div>

      {/* Face 5 — four corners + center */}
      <div className="dice-face face-5">
        <div className="dot dot-top-left" />
        <div className="dot dot-top-right" />
        <div className="dot dot-center" />
        <div className="dot dot-bottom-left" />
        <div className="dot dot-bottom-right" />
      </div>

      {/* Face 6 — two columns of three */}
      <div className="dice-face face-6">
        <div className="dot dot-top-left" />
        <div className="dot dot-top-right" />
        <div className="dot dot-mid-left" />
        <div className="dot dot-mid-right" />
        <div className="dot dot-bottom-left" />
        <div className="dot dot-bottom-right" />
      </div>
    </div>
  </div>
);

export default function Dice3D({ value, isRolling }) {
  const [faces, setFaces] = useState([1, 1]);

  useEffect(() => {
    if (!isRolling && value && value > 0) {
      Promise.resolve().then(() => {
        setFaces(splitDiceValue(value));
      });
    }
  }, [isRolling, value]);

  return (
    <div className="flex gap-4 items-center justify-center">
      <SingleDie face={faces[0]} isRolling={isRolling} delayOffset={0} />
      <SingleDie face={faces[1]} isRolling={isRolling} delayOffset={-200} />
    </div>
  );
}
