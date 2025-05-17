using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource windSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainTheme;

    [Header("Wind")]

    [Header("SFX Clips")]
    [SerializeField] private AudioClip grabSound;
    [SerializeField] private AudioClip[] cargoHitSounds;
    [SerializeField] private AudioClip windSound;
    [SerializeField] private AudioClip splash;


    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    public float SFXVolume => sfxVolume;
    public float MusicVolume => musicVolume;


    [SerializeField] private float windFadeDuration = 2f;
    [SerializeField] private float windMaxVolume = 0.6f;

    private Coroutine windFadeCoroutine;

    // void Update()
    // {
    //     Debug.Log("Music Volume: " + musicVolume.ToString());
    //     Debug.Log("SFX Volume: " + sfxVolume.ToString());   
    // }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load saved volume or default to 1
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    // Used in the UI in options
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    // Used in the UI in options
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }

    public void PlayMainTheme(bool loop = true)
    {
        PlayMusicClip(mainTheme, loop);
    }

    private void PlayMusicClip(AudioClip clip, bool loop)
    {
        musicSource.clip   = clip;
        musicSource.loop   = loop;
        musicSource.Play();
    }

    public void StopMusic(){
        musicSource.Stop();
    }

    public void PlayGrab() => PlaySfxClip(grabSound);

    private void PlaySfxClip(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
    public void PlaySfxClip(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }
    public void PlayRandomCargoHit(float volumeScale = 1f)
    {
        if (cargoHitSounds.Length == 0) return;
        AudioClip clip = cargoHitSounds[Random.Range(0, cargoHitSounds.Length)];
        PlaySfxClip(clip, volumeScale);
    }

    public void playSplash(float volumeScale = 1f)
    {
        PlaySfxClip(splash, volumeScale);
    }


    public void StartWind(float strength, float fadeDuration = -1f)
    {
        if (windSound == null || windSource == null) return;

        if (!windSource.isPlaying)
        {
            windSource.clip = windSound;
            windSource.loop = true;
            windSource.volume = 0f;
            windSource.Play();
        }

        if (windFadeCoroutine != null)
            StopCoroutine(windFadeCoroutine);

        float duration = (fadeDuration > 0f) ? fadeDuration : windFadeDuration;
        float targetVolume = Mathf.Clamp01(Mathf.Abs(strength)) * windMaxVolume * sfxVolume;

        windFadeCoroutine = StartCoroutine(FadeWind(windSource, windSource.volume, targetVolume, duration));
    }

    public void StopWind(float fadeDuration = -1f)
    {
        if (windSource == null) return;

        if (windFadeCoroutine != null)
            StopCoroutine(windFadeCoroutine);

        float duration = (fadeDuration > 0f) ? fadeDuration : windFadeDuration;

        windFadeCoroutine = StartCoroutine(FadeWind(windSource, windSource.volume, 0f, duration));
    }

    private IEnumerator FadeWind(AudioSource source, float from, float to, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            source.volume = Mathf.Lerp(from, to, t);
            yield return null;
        }

        source.volume = to;
    }



}
