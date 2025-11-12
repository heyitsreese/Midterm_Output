using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [Header("Panels")]
    public GameObject settingsPanel;

    [Header("Audio Settings")]
    public Slider musicSlider;
    public AudioSource musicSource;

    private const string MusicVolumeKey = "MusicVolume";

    void Start()
    {
        // Load saved volume (default to 1 if not found)
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);

        // Apply volume
        musicSource.volume = savedMusicVolume;

        // Update slider to match saved value
        if (musicSlider != null)
        {
            musicSlider.value = savedMusicVolume;
            // Add listener once
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        Debug.Log("Loaded saved volume: " + savedMusicVolume);
    }

    // opening cutscene plays first 
    public void PlayGame()
    {
        SceneManager.LoadScene("Opening");
    }

    public void OpenSettings()
    {
        if (settingsPanel)
            settingsPanel.SetActive(true);
    }
    
    public void CloseSettings()
    {
        if (settingsPanel)
            settingsPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void SetMusicVolume(float volume)
    {
        // Apply and save the new volume
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();

        Debug.Log("Set and saved volume: " + volume);
    }
}
