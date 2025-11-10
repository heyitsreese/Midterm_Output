using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuUI;
    public GameObject settingsPanel;

    public GameObject winCanvas;
    public GameObject lossCanvas;
    public PlayerController playerController;

    [Header("Audio")]
    public AudioSource clickSound;

    [Header("Player Scripts to Disable on Pause")]
    public MonoBehaviour[] scriptsToDisable;

    public static bool GameIsPaused { get; private set; } = false;

    private CanvasGroup pauseCanvasGroup;
    private CanvasGroup settingsCanvasGroup;

    void Start()
    {
        // Hide panels initially
        SetActiveImmediate(pauseMenuUI, false);
        SetActiveImmediate(settingsPanel, false);

        // Cache CanvasGroups for unscaled updates
        if (pauseMenuUI != null)
            pauseCanvasGroup = GetOrAddCanvasGroup(pauseMenuUI);
        if (settingsPanel != null)
            settingsCanvasGroup = GetOrAddCanvasGroup(settingsPanel);

        // Default cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("PauseMenu Start: " + gameObject.name);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        PlayClick();

        SetActiveImmediate(pauseMenuUI, true);
        SetActiveImmediate(settingsPanel, false);

        Time.timeScale = 0f;
        GameIsPaused = true;

        SetScriptsEnabled(false);

        playerController.SetCursorState(false);
    }

    public void ResumeGame()
    {
        PlayClick();

        SetActiveImmediate(pauseMenuUI, false);
        SetActiveImmediate(settingsPanel, false);

        Time.timeScale = 1f;
        GameIsPaused = false;

        SetScriptsEnabled(true);

        playerController.SetCursorState(true);
    }

    public void OpenSettings()
    {
        PlayClick();

        SetActiveImmediate(settingsPanel, true);
        SetActiveImmediate(pauseMenuUI, false);
    }

    public void CloseSettings()
    {
        PlayClick();

        SetActiveImmediate(settingsPanel, false);
        SetActiveImmediate(pauseMenuUI, true);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        GameIsPaused = false;
        SceneManager.LoadScene("StartMenu");
    }

    private void SetScriptsEnabled(bool enabled)
    {
        if (scriptsToDisable == null) return;

        foreach (var script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = enabled;
        }
    }

    private void PlayClick()
    {
        if (clickSound != null)
            clickSound.Play();
    }

    // Forces panel to activate and update even if Time.timeScale = 0
    private void SetActiveImmediate(GameObject panel, bool active)
    {
        if (panel == null) return;

        panel.SetActive(active);

        // Force Canvas to update immediately for pause-safe UI
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = active ? 1f : 0f;
            cg.interactable = active;
            cg.blocksRaycasts = active;
        }

        Canvas.ForceUpdateCanvases();
    }

    // Ensure the panel has a CanvasGroup
    private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = obj.AddComponent<CanvasGroup>();
        return cg;
    }

    public void ShowWin()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;

        SetActiveImmediate(winCanvas, true);
        SetActiveImmediate(lossCanvas, false);
        SetActiveImmediate(pauseMenuUI, false);
        SetActiveImmediate(settingsPanel, false);

        SetScriptsEnabled(false);

        playerController.SetCursorState(false);
    }

    public void ShowLoss()
    {
        Time.timeScale = 0f;
        GameIsPaused = true;

        SetActiveImmediate(lossCanvas, true);
        SetActiveImmediate(winCanvas, false);
        SetActiveImmediate(pauseMenuUI, false);
        SetActiveImmediate(settingsPanel, false);

        SetScriptsEnabled(false);

        playerController.SetCursorState(false);
    }

    public void PlayAgain()
    {
        PlayClick();

        Time.timeScale = 1f;
        GameIsPaused = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
