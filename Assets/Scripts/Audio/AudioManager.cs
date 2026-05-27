using UnityEngine;

namespace Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;

        [Header("BGM Clips")]
        public AudioClip mainMenuBGM;
        public AudioClip gameplayBGM;

        [Header("SFX Clips")]
        public AudioClip clickClip;
        public AudioClip diceRollClip;
        public AudioClip tokenMoveClip;
        public AudioClip ladderClip;
        public AudioClip snakeClip;
        public AudioClip skullClip;
        public AudioClip quizCorrectClip;
        public AudioClip quizWrongClip;
        public AudioClip winClip;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.volume = 0.5f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.volume = 0.8f;
            }
        }

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null || musicSource == null) return;

            if (musicSource.clip == clip && musicSource.isPlaying)
                return;

            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopBGM()
        {
            if (musicSource != null) musicSource.Stop();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip);
        }
    }
}
