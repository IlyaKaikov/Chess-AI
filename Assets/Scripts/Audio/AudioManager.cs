namespace ChessAI.Audio
{
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Sound Effects")]
        public AudioClip gameStartSound;
        public AudioClip movePieceSound;
        public AudioClip illegal;
        public AudioClip capturePieceSound;
        public AudioClip castleSound;
        public AudioClip promotionSound;
        public AudioClip checkSound;
        public AudioClip gameEndSound;

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlaySound(AudioClip clip)
        {
            AudioSource tempAudioSource = gameObject.AddComponent<AudioSource>();
            tempAudioSource.clip = clip;
            tempAudioSource.Play();
            Destroy(tempAudioSource, clip.length);
        }
    }
}
