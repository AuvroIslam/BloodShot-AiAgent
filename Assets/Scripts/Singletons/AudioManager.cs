using UnityEngine;
using SingletonManager;
using System.Collections.Generic;

namespace Singletons
{
    public class AudioManager : SingletonPersistent
    {
        public static AudioManager Instance => GetInstance<AudioManager>();

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public bool loop = false;
            public bool isBGM = false; // Is this background music?
        }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Sound Library")]
        [SerializeField] private List<Sound> _sounds;

        private Dictionary<string, Sound> _soundDictionary = new Dictionary<string, Sound>();
        
        // Volume settings (0-1)
        private float _bgmVolume = 0.5f;
        private float _sfxVolume = 0.7f;

        public float BGMVolume 
        { 
            get => _bgmVolume;
            set
            {
                _bgmVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat("BGMVolume", _bgmVolume);
                UpdateBGMVolume();
            }
        }

        public float SFXVolume 
        { 
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            // Create audio sources if they don't exist
            if (_bgmSource == null)
            {
                GameObject bgmObj = new GameObject("BGM_AudioSource");
                bgmObj.transform.SetParent(transform);
                _bgmSource = bgmObj.AddComponent<AudioSource>();
                _bgmSource.loop = true;
                _bgmSource.playOnAwake = false;
            }

            if (_sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFX_AudioSource");
                sfxObj.transform.SetParent(transform);
                _sfxSource = sfxObj.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            // Build sound dictionary
            foreach (var sound in _sounds)
            {
                if (sound.clip != null)
                {
                    _soundDictionary[sound.name] = sound;
                }
            }

            // Load saved volumes
            _bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
            UpdateBGMVolume();
        }

        /// <summary>
        /// Play a sound effect once
        /// </summary>
        public void PlaySFX(string soundName)
        {
            if (!_soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"Sound '{soundName}' not found!");
                return;
            }

            Sound sound = _soundDictionary[soundName];
            if (sound.isBGM)
            {
                Debug.LogWarning($"Sound '{soundName}' is BGM, use PlayBGM instead!");
                return;
            }

            _sfxSource.PlayOneShot(sound.clip, sound.volume * _sfxVolume);
        }

        /// <summary>
        /// Play background music (will loop)
        /// </summary>
        public void PlayBGM(string soundName)
        {
            if (!_soundDictionary.ContainsKey(soundName))
            {
                Debug.LogWarning($"BGM '{soundName}' not found!");
                return;
            }

            Sound sound = _soundDictionary[soundName];
            
            // Don't restart if already playing the same BGM
            if (_bgmSource.isPlaying && _bgmSource.clip == sound.clip)
                return;

            // Stop current BGM before switching
            _bgmSource.Stop();
            _bgmSource.clip = sound.clip;
            _bgmSource.volume = sound.volume * _bgmVolume;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        /// <summary>
        /// Stop background music
        /// </summary>
        public void StopBGM()
        {
            _bgmSource.Stop();
        }

        /// <summary>
        /// Pause background music
        /// </summary>
        public void PauseBGM()
        {
            _bgmSource.Pause();
        }

        /// <summary>
        /// Resume background music
        /// </summary>
        public void ResumeBGM()
        {
            _bgmSource.UnPause();
        }

        private void UpdateBGMVolume()
        {
            if (_bgmSource != null && _bgmSource.clip != null)
            {
                string currentBGM = "";
                foreach (var kvp in _soundDictionary)
                {
                    if (kvp.Value.clip == _bgmSource.clip)
                    {
                        currentBGM = kvp.Key;
                        break;
                    }
                }

                if (!string.IsNullOrEmpty(currentBGM))
                {
                    _bgmSource.volume = _soundDictionary[currentBGM].volume * _bgmVolume;
                }
            }
        }
    }
}
