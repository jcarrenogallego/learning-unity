using UnityEngine;

namespace Kogi.Scripts.Audio
{
    public sealed class GameAudioManager : MonoBehaviour
    {
        private const string MusicVolumeKey = "Audio.MusicVolume";
        private const string EffectsVolumeKey = "Audio.EffectsVolume";

        public static GameAudioManager Instance { get; private set; }
        public float MusicVolume { get; private set; }
        public float EffectsVolume { get; private set; }

        private AudioSource musicSource;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void CreateInstance()
        {
            if (Instance != null)
            {
                return;
            }

            GameObject manager = new GameObject("GameAudioManager");
            DontDestroyOnLoad(manager);
            Instance = manager.AddComponent<GameAudioManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            MusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.32f);
            EffectsVolume = PlayerPrefs.GetFloat(EffectsVolumeKey, 0.8f);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.spatialBlend = 0f;
            musicSource.ignoreListenerPause = false;
            musicSource.clip = CreateAmbientLoop();
            ApplyVolumes();
            musicSource.Play();
        }

        public void SetMusicVolume(float value)
        {
            MusicVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(MusicVolumeKey, MusicVolume);
            PlayerPrefs.Save();
            ApplyVolumes();
        }

        public void SetEffectsVolume(float value)
        {
            EffectsVolume = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(EffectsVolumeKey, EffectsVolume);
            PlayerPrefs.Save();
        }

        private void ApplyVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = MusicVolume;
            }
        }

        private static AudioClip CreateAmbientLoop()
        {
            const int sampleRate = 22050;
            const int durationSeconds = 8;
            int sampleCount = sampleRate * durationSeconds;
            float[] samples = new float[sampleCount];

            for (int index = 0; index < sampleCount; index++)
            {
                float time = (float)index / sampleRate;
                float fade = Mathf.Sin(Mathf.PI * index / sampleCount);
                float lowPad = Mathf.Sin(2f * Mathf.PI * 55f * time) * 0.055f;
                float highPad = Mathf.Sin(2f * Mathf.PI * 82.5f * time) * 0.025f;
                samples[index] = (lowPad + highPad) * (0.55f + 0.45f * fade);
            }

            AudioClip clip = AudioClip.Create("DesertNight_Ambient", sampleCount, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
