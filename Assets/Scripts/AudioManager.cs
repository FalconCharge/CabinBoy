using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // 1) Singleton so you can call AudioManager.Instance anywhere
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainTheme;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip grab;

    private void Awake()
    {
        // basic singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- Music API ---
    public void PlayMainTheme(float volume = 1f, bool loop = true)
    {
        PlayMusicClip(mainTheme, volume, loop);
    }

    private void PlayMusicClip(AudioClip clip, float volume, bool loop)
    {
        if (clip == null) return;
        musicSource.clip   = clip;
        musicSource.volume = volume;
        musicSource.loop   = loop;
        musicSource.Play();
    }

    public void StopMusic(){
        musicSource.Stop();
    }

    // --- One‐shot SFX API ---
    public void PlayGrab() => PlaySfxClip(grab);

    private void PlaySfxClip(AudioClip clip)
    {
        if (clip == null) return;
        
        sfxSource.PlayOneShot(clip);
    }
}
