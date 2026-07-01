import { useState, useCallback, useEffect } from 'react';
import { AudioContext } from './AudioContext';
import audioManager from '../audio/AudioManager';

export function AudioProvider({ children }) {
  const [muted, setMutedState] = useState(audioManager.muted);

  useEffect(() => {
    // Initial preload of audio files
    audioManager.preload();

    // Auto-unlock audio on the very first user interaction anywhere on the window
    const handleFirstGesture = () => {
      audioManager.unlockAudio().then(() => {
        // If not muted, start BGM on first gesture
        if (!audioManager.muted) {
          audioManager.playBgm();
        }
      });
      window.removeEventListener('click', handleFirstGesture, true);
      window.removeEventListener('keydown', handleFirstGesture, true);
      window.removeEventListener('touchstart', handleFirstGesture, true);
    };

    window.addEventListener('click', handleFirstGesture, true);
    window.addEventListener('keydown', handleFirstGesture, true);
    window.addEventListener('touchstart', handleFirstGesture, true);

    return () => {
      window.removeEventListener('click', handleFirstGesture, true);
      window.removeEventListener('keydown', handleFirstGesture, true);
      window.removeEventListener('touchstart', handleFirstGesture, true);
      audioManager.cleanup();
    };
  }, []);

  const toggleMute = useCallback(() => {
    const nextMuted = !muted;
    audioManager.setMuted(nextMuted);
    setMutedState(nextMuted);
  }, [muted]);

  const playSfx = useCallback((name) => {
    audioManager.playSfx(name);
  }, []);

  const playBgm = useCallback(() => {
    audioManager.playBgm();
  }, []);

  const stopBgm = useCallback(() => {
    audioManager.stopBgm();
  }, []);

  const unlockAudio = useCallback(async () => {
    await audioManager.unlockAudio();
  }, []);

  const setBgmVolume = useCallback((volume) => {
    audioManager.setBgmVolume(volume);
  }, []);

  const setSfxVolume = useCallback((volume) => {
    audioManager.setSfxVolume(volume);
  }, []);

  return (
    <AudioContext.Provider value={{
      muted,
      toggleMute,
      playSfx,
      playBgm,
      stopBgm,
      unlockAudio,
      setBgmVolume,
      setSfxVolume
    }}>
      {children}
    </AudioContext.Provider>
  );
}
