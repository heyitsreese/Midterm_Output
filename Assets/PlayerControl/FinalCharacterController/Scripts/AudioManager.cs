using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Music Sources")]
    public AudioSource normalMusicSource;
    public AudioSource fearMusicSource;

    [Header("SFX Sources")]
    public AudioSource footstepsSource;
    public AudioSource whisper1Source;
    public AudioSource whisper2Source;
    public AudioClip clickSound;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load saved volumes
        SetMusicVolume(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        SetSFXVolume(PlayerPrefs.GetFloat(SFXVolumeKey, 1f));
    }

    // ================= MUSIC ================= //
    public void PlayNormalMusic()
    {
        fearMusicSource?.Stop();
        normalMusicSource?.Play();
    }

    public void PlayFearMusic()
    {
        normalMusicSource?.Stop();
        fearMusicSource?.Play();
    }

    public void StopAllMusic()
    {
        normalMusicSource?.Stop();
        fearMusicSource?.Stop();
    }

    public void SetMusicVolume(float value)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("MusicVolume", dB);
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
        }
    }

    public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

    // ================= SFX ================= //

    public void PlayClickSound()
    {
        if (clickSound != null)
            footstepsSource?.PlayOneShot(clickSound); // use footstepsSource for generic SFX
    }

    public void PlayFootsteps()
    {
        footstepsSource?.Play();
    }

    public void StopFootsteps()
    {
        footstepsSource?.Stop();
    }

    public void PlayWhisper1()
    {
        if (whisper1Source != null && !whisper1Source.isPlaying)
            whisper1Source.Play();
    }

    public void PlayWhisper2()
    {
        if (whisper2Source != null && !whisper2Source.isPlaying)
            whisper2Source.Play();
    }

    public void StopWhispers()
    {
        whisper1Source?.Stop();
        whisper2Source?.Stop();
    }

    public void SetSFXVolume(float value)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("SFXVolume", dB);
            PlayerPrefs.SetFloat(SFXVolumeKey, value);

            // Also set volume of individual SFX sources
            footstepsSource.volume = value;
            whisper1Source.volume = value;
            whisper2Source.volume = value;
        }
    }

    public float GetSFXVolume() => PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
}
