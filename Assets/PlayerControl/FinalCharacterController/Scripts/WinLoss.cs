using UnityEngine;

public class WinLoss : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winCanvas;
    public GameObject lossCanvas; 
    public PauseMenu pauseMenu;

    private bool gameEnded = false;

    void Start()
    {
        if (winCanvas != null) winCanvas.SetActive(false);
        if (lossCanvas != null) lossCanvas.SetActive(false);
    }

    public void ShowWinScreen()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f; // pause the game

        if (winCanvas != null)
            winCanvas.SetActive(true);

        Debug.Log("🎉 Player WON!");

        pauseMenu.playerController.SetCursorState(false);
    }

    public void ShowLossScreen()
    {
        if (gameEnded) return;
        gameEnded = true;

        Time.timeScale = 0f; // pause the game

        if (lossCanvas != null)
            lossCanvas.SetActive(true);
        else
            Debug.LogWarning("⚠️ Loss Canvas not assigned!");

        Debug.Log("💀 Player LOST!");

        pauseMenu.playerController.SetCursorState(false);
    }
}
