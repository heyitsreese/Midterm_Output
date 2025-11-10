using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip clickSound;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        // Singleton pattern
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
        float musicVol = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVol = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        SetMusicVolume(musicVol);
        SetSFXVolume(sfxVol);
    }

    public void PlayClickSound()
    {
        if (clickSound != null && sfxSource != null)
            sfxSource.PlayOneShot(clickSound);
    }

    public void SetMusicVolume(float value)
    {
        if (audioMixer != null)
        {
            // Convert linear (0–1) to decibels (-80 dB to 0 dB)
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("MusicVolume", dB);
            PlayerPrefs.SetFloat(MusicVolumeKey, value);
        }
    }

    public void SetSFXVolume(float value)
    {
        if (audioMixer != null)
        {
            float dB = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
            audioMixer.SetFloat("SFXVolume", dB);
            PlayerPrefs.SetFloat(SFXVolumeKey, value);
        }
    }

    public float GetMusicVolume() => PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
    public float GetSFXVolume() => PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
}
