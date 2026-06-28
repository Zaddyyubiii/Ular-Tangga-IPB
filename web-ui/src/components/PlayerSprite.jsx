import { useState, useEffect, useRef } from 'react';

const getColorName = (hex) => {
  if (!hex) return 'blue';
  const h = hex.toLowerCase();
  if (h.includes('f443') || h.includes('red') || h.startsWith('#f') || h.startsWith('#e')) return 'red';
  if (h.includes('4caf') || h.includes('green') || h.startsWith('#4') || h.startsWith('#3')) return 'green';
  if (h.includes('ffeb') || h.includes('yellow') || h.includes('#ffc') || h.includes('#ffd')) return 'yellow trans';
  return 'blue';
};

export default function PlayerSprite({ hexColor, stage, currentTile }) {
  const color = getColorName(hexColor);
  const [pose, setPose] = useState('c1'); // c1=idle, c2=walkR, c3=walkMid, c4=walkL
  const [currentRow, setCurrentRow] = useState(stage || 1);
  const prevTile = useRef(currentTile);
  const animTimer = useRef(null);
  const resetTimer = useRef(null);

  useEffect(() => {
    if (prevTile.current === currentTile) return;
    const diff = currentTile - prevTile.current;
    prevTile.current = currentTile;

    clearInterval(animTimer.current);
    clearTimeout(resetTimer.current);

    const resetToIdle = () => {
      setCurrentRow(stage || 1);
      setPose('c1');
    };

    if (diff > 6) { // Ladder (goes up)
      setCurrentRow(6);
      setPose('c1');
      resetTimer.current = setTimeout(resetToIdle, 2000);
      return;
    }

    if (diff < 0) { // Snake (goes down)
      setCurrentRow(6);
      setPose('c2');
      resetTimer.current = setTimeout(resetToIdle, 2000);
      return;
    }

    // Walking animation
    setCurrentRow(stage || 1);
    let frame = 0;
    const frames = ['c2', 'c3', 'c4', 'c3'];
    animTimer.current = setInterval(() => {
      setPose(frames[frame % 4]);
      frame++;
    }, 200);

    resetTimer.current = setTimeout(() => {
      clearInterval(animTimer.current);
      resetToIdle();
    }, Math.min(diff * 200, 1500)); // Walk duration scales with distance

    return () => {
      clearInterval(animTimer.current);
      clearTimeout(resetTimer.current);
    };
  }, [currentTile, stage]);

  useEffect(() => {
    if (currentRow !== 6) setCurrentRow(stage || 1);
  }, [stage, currentRow]);

  const prefix = color === 'blue' ? 'blue__' : `${color}_`;
  const src = `/sprites/${prefix}split_r${currentRow}_${pose}.png`;

  return (
    <img
      src={src}
      className="w-16 h-16 object-contain"
      style={{ imageRendering: 'pixelated' }}
      alt={`Player sprite - ${color}`}
    />
  );
}