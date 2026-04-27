using UnityEngine;
using UnityEngine.UI;
using Singletons;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioSettingsUI: AudioManager not found!");
            return;
        }

        // Initialize sliders with current volume values
        if (_bgmSlider != null)
        {
            _bgmSlider.value = AudioManager.Instance.BGMVolume;
            _bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = AudioManager.Instance.SFXVolume;
            _sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void OnDestroy()
    {
        if (_bgmSlider != null)
            _bgmSlider.onValueChanged.RemoveListener(OnBGMVolumeChanged);
        
        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    private void OnBGMVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BGMVolume = value;
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SFXVolume = value;
        }
    }
}
