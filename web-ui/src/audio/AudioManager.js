class AudioManager {
  constructor() {
    this.muted = localStorage.getItem('game_audio_muted') === 'true';
    this.bgmVolume = parseFloat(localStorage.getItem('game_bgm_volume') || '0.35');
    this.sfxVolume = parseFloat(localStorage.getItem('game_sfx_volume') || '0.5');
    
    this.bgmInstance = null;
    this.sfxPools = {};
    this.maxPoolSize = 5;
    
    this.sfxPaths = {
      click: './audio/sfx/Click.wav',
      dice: './audio/sfx/Dice.mp3',
      movePetak: './audio/sfx/MovePetak.wav',
      moveStep: './audio/sfx/MovePetak.wav',
      petakBonus: './audio/sfx/PetakBonus.wav',
      ladderBonus: './audio/sfx/PetakBonus.wav',
      quizCorrect: './audio/sfx/PetakBonus.wav',
      petakPunishment: './audio/sfx/PetakPunishment.wav',
      punishment: './audio/sfx/PetakPunishment.wav',
      victory: './audio/sfx/Victory_pake_yang_ini_aja.wav',
      winAlt1: './audio/sfx/win_sound_2_1.wav',
      winAlt2: './audio/sfx/win_sound_2_3.wav',
      winAlt: './audio/sfx/win_sound_2_3.wav'
    };
    this.sfxFallbacks = {
      victory: ['winAlt1', 'winAlt2']
    };
    
    this.bgmPath = './audio/bgm/Superhero_violin_no_intro.ogg';
    
    this.isUnlocked = false;
    this.lastPlayTime = {}; // Cooldown tracking (e.g. for movePetak)
  }

  // Preload files
  preload() {
    try {
      // Preload BGM
      if (!this.bgmInstance) {
        this.bgmInstance = new Audio(this.bgmPath);
        this.bgmInstance.loop = true;
        this.bgmInstance.volume = this.muted ? 0 : this.bgmVolume;
        this.bgmInstance.preload = 'auto';
      }
      
      // Preload single instance for each SFX
      Object.keys(this.sfxPaths).forEach(name => {
        if (!this.sfxPools[name]) {
          const audio = new Audio(this.sfxPaths[name]);
          audio.preload = 'auto';
          audio.volume = this.sfxVolume;
          this.sfxPools[name] = [audio];
        }
      });
    } catch (err) {
      console.warn("AudioManager preload failed:", err);
    }
  }

  async unlockAudio() {
    if (this.isUnlocked) return;
    try {
      if (!this.bgmInstance) {
        this.preload();
      }
      if (this.bgmInstance) {
        const originalVolume = this.bgmInstance.volume;
        this.bgmInstance.volume = 0;
        await this.bgmInstance.play();
        this.bgmInstance.pause();
        this.bgmInstance.volume = originalVolume;
      }
      this.isUnlocked = true;
      console.log("Audio context unlocked successfully.");
    } catch (err) {
      console.warn("Audio unlock failed:", err);
    }
  }

  playBgm() {
    if (!this.bgmInstance) {
      this.preload();
    }

    if (!this.bgmInstance) return;
    
    if (this.muted) {
      this.bgmInstance.volume = 0;
    } else {
      this.bgmInstance.volume = this.bgmVolume;
    }

    if (!this.bgmInstance.paused) return;

    this.bgmInstance.play().catch(err => {
      console.warn("BGM play failed, waiting for user interaction:", err);
    });
  }

  stopBgm() {
    if (this.bgmInstance) {
      this.bgmInstance.pause();
      this.bgmInstance.currentTime = 0;
    }
  }

  pauseBgm() {
    if (this.bgmInstance) {
      this.bgmInstance.pause();
    }
  }

  resumeBgm() {
    if (this.bgmInstance && !this.muted) {
      this.bgmInstance.play().catch(err => {
        console.warn("BGM resume failed:", err);
      });
    }
  }

  playSfx(name) {
    // Cooldown check for rapid movement sound (e.g. movePetak)
    const now = Date.now();
    if (name === 'moveStep' || name === 'movePetak') {
      const lastTime = this.lastPlayTime[name] || 0;
      if (now - lastTime < 40) { // 40ms cooldown for smooth cascading steps
        return;
      }
      this.lastPlayTime[name] = now;
    }

    if (this.muted) return;

    const path = this.sfxPaths[name];
    if (!path) {
      console.warn(`SFX '${name}' not mapped.`);
      return;
    }

    if (!this.sfxPools[name]) {
      this.sfxPools[name] = [];
    }

    const pool = this.sfxPools[name];
    
    // Find an idle audio element in the pool
    let audio = pool.find(a => a.paused || a.ended);
    
    if (!audio) {
      if (pool.length < this.maxPoolSize) {
        // Create new instance and add to pool
        try {
          audio = new Audio(path);
          audio.preload = 'auto';
          pool.push(audio);
        } catch (err) {
          console.warn(`Failed to create Audio instance for SFX '${name}':`, err);
          return;
        }
      } else {
        // Pool is full, reuse the oldest (first in pool)
        audio = pool[0];
        audio.pause();
        audio.currentTime = 0;
      }
    }

    if (audio) {
      audio.volume = this.sfxVolume;
      audio.currentTime = 0;
      audio.play().catch(err => {
        console.warn(`SFX '${name}' play failed:`, err);
        this.playFallbackSfx(name);
      });
    }
  }

  playFallbackSfx(name) {
    const fallbackNames = this.sfxFallbacks[name] || [];
    const fallback = fallbackNames.find(fallbackName => this.sfxPaths[fallbackName]);
    if (fallback) {
      this.playSfx(fallback);
    }
  }

  setMuted(muted) {
    this.muted = muted;
    localStorage.setItem('game_audio_muted', muted ? 'true' : 'false');
    
    if (this.bgmInstance) {
      if (muted) {
        this.bgmInstance.volume = 0;
        this.bgmInstance.pause();
      } else {
        this.bgmInstance.volume = this.bgmVolume;
        this.bgmInstance.play().catch(err => {
          console.warn("BGM resume after unmute failed:", err);
        });
      }
    }
  }

  setBgmVolume(volume) {
    this.bgmVolume = volume;
    localStorage.setItem('game_bgm_volume', volume.toString());
    if (this.bgmInstance && !this.muted) {
      this.bgmInstance.volume = volume;
    }
  }

  setSfxVolume(volume) {
    this.sfxVolume = volume;
    localStorage.setItem('game_sfx_volume', volume.toString());
    // Update existing pool elements volume
    Object.keys(this.sfxPools).forEach(name => {
      this.sfxPools[name].forEach(audio => {
        audio.volume = volume;
      });
    });
  }

  cleanup() {
    this.stopBgm();
    if (this.bgmInstance) {
      this.bgmInstance.src = '';
      this.bgmInstance = null;
    }
    Object.keys(this.sfxPools).forEach(name => {
      this.sfxPools[name].forEach(audio => {
        audio.pause();
        audio.src = '';
      });
      this.sfxPools[name] = [];
    });
    this.isUnlocked = false;
  }
}

const managerInstance = new AudioManager();
export default managerInstance;
