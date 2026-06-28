import { useState, useEffect, useRef } from 'react';

const getPlayerId = (hex) => {
  if (!hex) return 2;
  const h = hex.toLowerCase();
  if (h.includes('9e2f2f') || h.includes('e84c4c') || h.includes('e63333')) return 1; // red
  if (h.includes('2f5da8') || h.includes('4f8cff') || h.includes('3366e6')) return 2; // blue
  if (h.includes('2f8b57') || h.includes('38d27a') || h.includes('1ebb59')) return 3; // green
  if (h.includes('b87822') || h.includes('ffc247') || h.includes('f2bf26')) return 4; // yellow
  return 2;
};

const CACHE_BUST = Date.now();

export default function PlayerSprite({ playerId, hexColor, stage, currentTile }) {
  const actualPlayerId = playerId || getPlayerId(hexColor);

  // Mapping yg benar berdasarkan gambar referensi:
  // Column (c1-c4) = Stage (1: Punk, 2: Mulai Belajar, 3: Tertib, 4: Teladan)
  // Row (r1-r6) = Animasi (1: Idle, 2: Walk1, 3: Walk2, 4: Walk3, 5: Happy, 6: Surprised)
  const col = Math.max(1, Math.min(stage || 1, 4));
  const [row, setRow] = useState(1); // r1 = idle

  const prevTile = useRef(currentTile);
  const animTimer = useRef(null);
  const resetTimer = useRef(null);

  useEffect(() => {
    if (prevTile.current === currentTile) return;
    const diff = currentTile - prevTile.current;
    prevTile.current = currentTile;

    clearInterval(animTimer.current);
    clearTimeout(resetTimer.current);

    const resetToIdle = () => setRow(1);

    if (diff > 6) { // Naik Tangga -> Happy (r5)
      setRow(5);
      resetTimer.current = setTimeout(resetToIdle, 2000);
      return;
    }

    if (diff < 0) { // Kena Ular -> Shocked (r6)
      setRow(6);
      resetTimer.current = setTimeout(resetToIdle, 2000);
      return;
    }

    // Animasi Jalan -> r2, r3, r4
    let frame = 0;
    const walkFrames = [2, 3, 4, 3];
    animTimer.current = setInterval(() => {
      setRow(walkFrames[frame % 4]);
      frame++;
    }, 200);

    resetTimer.current = setTimeout(() => {
      clearInterval(animTimer.current);
      resetToIdle();
    }, Math.min(Math.abs(diff) * 200, 1500));

    return () => {
      clearInterval(animTimer.current);
      clearTimeout(resetTimer.current);
    };
  }, [currentTile]);

  return (
    <div className="relative w-full h-full flex items-center justify-center">
      {[1, 2, 3, 4, 5, 6].map(r => {
        const filename = `p${actualPlayerId}_r${r}_c${col}.png`;
        const src = `./sprites/${filename}?v=${CACHE_BUST}`;
        const isVisible = r === row;
        
        return (
          <img
            key={r}
            src={src}
            className="absolute w-[110%] h-[110%] object-contain transition-opacity duration-75"
            style={{
              imageRendering: 'pixelated',
              transformOrigin: 'center',
              opacity: isVisible ? 1 : 0,
              pointerEvents: isVisible ? 'auto' : 'none'
            }}
            alt={isVisible ? `Character Stage ${col}` : ""}
          />
        );
      })}
    </div>
  );
}