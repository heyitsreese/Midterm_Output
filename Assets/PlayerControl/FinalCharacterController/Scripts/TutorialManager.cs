using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;
    public TMP_Text continuePromptText;

    [Header("Tutorial Settings")]
    public float typingSpeed = 0.03f;
    public float blinkSpeed = 1.5f; // speed of fade in/out

    private int tutorialStep = 0;
    private bool hasPickedUpItem = false;
    private bool isStoryPlaying = false;
    private bool isBlinking = false;

    private DayNightCycle dayNightCycle;
    private PlayerController playerController;
    private GameObject door;
    public GameObject key;

    // Track which movement keys were pressed
    private bool pressedW, pressedA, pressedS, pressedD;

    // Storyline messages
    private readonly string[] storyMessages =
    {
        "The forest of Thalara was once a place of harmony between humans and spirits.",
        "Careless visitors began to litter, angering the spirits. Thalara became cursed.",
        "Lumi and friends ignored the warnings, driving deep into the foggy forest.",
        "Their vehicle broke down. Alone, Lumi ventured on foot, noticing scattered trash.",
        "She began cleaning, following bins along the path. Each correct choice calmed the forest.",
        "Mistakes made the forest menacing, its whispers and footsteps growing louder.",
    };

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        dayNightCycle = FindFirstObjectByType<DayNightCycle>();

        if (dayNightCycle != null)
            dayNightCycle.enabled = false;

        door = GameObject.FindWithTag("Door");
        if (door != null)
            door.SetActive(false);

        if (key != null)
            key.SetActive(false);

        if (continuePromptText != null)
            continuePromptText.gameObject.SetActive(false);

        // Lock player movement at the start of the tutorial
        if (playerController != null)
        {
            playerController.canMove = true;  // allow moving for the movement tutorial
            playerController.canJump = true;  // allow jumping later (disabled during story)
        }

        ShowMessage("Use <b>W/A/S/D</b> to move around.");
    }

    void Update()
    {
        if (isStoryPlaying) return; // Stop tutorial logic while story is playing

        switch (tutorialStep)
        {
            case 0:
                if (PlayerHasMovedAllDirections())
                {
                    tutorialStep++;
                    ShowMessage("Press <b>E</b> to pick up items.");
                }
                break;

            case 1:
                if (hasPickedUpItem)
                {
                    tutorialStep++;
                    StartCoroutine(PlayStoryWithSpace());
                }
                break;
        }
    }

    bool PlayerHasMovedAllDirections()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return false;

        if (keyboard.wKey.wasPressedThisFrame) pressedW = true;
        if (keyboard.aKey.wasPressedThisFrame) pressedA = true;
        if (keyboard.sKey.wasPressedThisFrame) pressedS = true;
        if (keyboard.dKey.wasPressedThisFrame) pressedD = true;

        return pressedW && pressedA && pressedS && pressedD;
    }

    public void ShowMessage(string message)
    {
        tutorialPanel.SetActive(true);
        tutorialText.text = message;
    }

    public void NotifyItemPickedUp()
    {
        hasPickedUpItem = true;
    }

    IEnumerator PlayStoryWithSpace()
    {
        isStoryPlaying = true;
        tutorialPanel.SetActive(true);

        // Disable movement & jumping during story
        if (playerController != null)
        {
            playerController.canMove = false;
            playerController.canJump = false;
        }

        for (int i = 0; i < storyMessages.Length; i++)
        {
            // Hide prompt while typing
            if (continuePromptText != null)
                continuePromptText.gameObject.SetActive(false);

            yield return StartCoroutine(TypeText(storyMessages[i]));

            // Show and start blinking "Press Space"
            if (continuePromptText != null)
            {
                continuePromptText.gameObject.SetActive(true);
                StartBlinking();
            }

            // Wait for space key
            yield return new WaitUntil(() => Keyboard.current.spaceKey.wasPressedThisFrame);

            StopBlinking();
        }

        // End story
        if (continuePromptText != null)
            continuePromptText.gameObject.SetActive(false);

        HideTutorial();
    }

    IEnumerator TypeText(string message)
    {
        tutorialText.text = "";
        foreach (char c in message)
        {
            tutorialText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    void StartBlinking()
    {
        if (!isBlinking && continuePromptText != null)
            StartCoroutine(BlinkText());
    }

    void StopBlinking()
    {
        isBlinking = false;
        if (continuePromptText != null)
            continuePromptText.alpha = 1f; // ensure visible after stop
    }

    IEnumerator BlinkText()
    {
        isBlinking = true;
        float t = 0f;

        while (isBlinking)
        {
            t += Time.unscaledDeltaTime * blinkSpeed;
            float alpha = (Mathf.Sin(t) + 1f) / 2f; // fade in/out between 0–1
            continuePromptText.alpha = Mathf.Lerp(0.2f, 1f, alpha);
            yield return null;
        }
    }

    void HideTutorial()
    {
        tutorialPanel.SetActive(false);

        // Re-enable player controls after story
        if (playerController != null)
        {
            playerController.canMove = true;
            playerController.canJump = true;
        }

        if (dayNightCycle != null)
            dayNightCycle.enabled = true;

        isStoryPlaying = false;
        this.enabled = false;
    }

    public void HideMessage()
    {
        tutorialText.text = ""; 
        tutorialPanel.SetActive(false);
    }
}
