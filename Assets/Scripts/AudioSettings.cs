using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        sfxSlider.value = AudioManager.Instance.SFXVolume;
        musicSlider.value = AudioManager.Instance.MusicVolume;

        // Add listeners
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void SetSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void SetMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnDestroy()
    {
        sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
        musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
    }
}
