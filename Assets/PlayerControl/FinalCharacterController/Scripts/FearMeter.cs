// using UnityEngine;
// using UnityEngine.UI;

// public class FearMeter : MonoBehaviour
// {
//     [Header("Fear Settings")]
//     public float fear = 0f;
//     public float maxFear = 100f;
//     public float increaseRate = 5f;   // Fear rises faster at night outside safe zone
//     public float decreaseRate = 2f;   // Fear slowly drops in safe zone or daytime
//     public bool isInSafeZone = true;

//     [Header("Day-Night Reference")]
//     public DayNightCycle dayNightCycle; // Drag your DayNightCycle GameObject here

//     [Header("UI")]
//     public Slider fearSlider;
//     public Image fillImage;
//     public Color safeColor = Color.green;
//     public Color dangerColor = Color.red;

//     void Start()
//     {
//         if (fearSlider != null)
//         {
//             fearSlider.maxValue = maxFear;
//             fearSlider.value = fear;
//         }
//     }

//     void Update()
//     {
//         if (dayNightCycle == null)
//         {
//             Debug.LogWarning("⚠️ DayNightCycle reference missing!");
//             return;
//         }

//         bool isNight = dayNightCycle.IsNight; // ✅ Uses your public getter

//         if (!isInSafeZone && isNight)
//             fear += increaseRate * Time.deltaTime;
//         else
//             fear -= decreaseRate * Time.deltaTime;

//         fear = Mathf.Clamp(fear, 0, maxFear);

//         if (fearSlider != null)
//         {
//             fearSlider.value = fear;
//             if (fillImage != null)
//                 fillImage.color = Color.Lerp(safeColor, dangerColor, fear / maxFear);
//         }

//         if (fear >= maxFear)
//         {
//             Debug.Log("😱 Fear maxed out! The player is in danger!");
//             // Optional: trigger death, fainting, or hallucination
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("SafeZone"))
//         {
//             isInSafeZone = true;
//             Debug.Log("🟢 Entered Safe Zone");
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("SafeZone"))
//         {
//             isInSafeZone = false;
//             Debug.Log("🔴 Left Safe Zone");
//         }
//     }
// }

using UnityEngine;
using UnityEngine.UI;

public class FearMeter : MonoBehaviour
{
    [Header("Fear Settings")]
    public float fear = 0f;
    public float maxFear = 100f;
    public float increaseRate = 5f;
    public float decreaseRate = 2f;
    public bool isInSafeZone = false;

    [Header("Day-Night Reference")]
    public DayNightCycle dayNightCycle;

    [Header("UI")]
    public Slider fearSlider;
    public Image fillImage;
    public Color safeColor = Color.green;
    public Color dangerColor = Color.red;

    [Header("Fear Effects")]
    public Camera playerCamera;
    public float shakeMagnitude = 0.1f;
    public float shakeSpeed = 5f;
    public Image hazeOverlay; // semi-transparent UI panel for haze effect
    public float hazeMaxAlpha = 0.5f;

    [Header("Messages")]
    public TutorialManager tutorialManager; // Assign your TutorialManager here
    private bool hasTriggeredHallucinationMessage = false;

    private Vector3 originalCamPos;

    void Start()
    {
        if (fearSlider != null)
        {
            fearSlider.maxValue = maxFear;
            fearSlider.value = fear;
        }

        if (playerCamera != null)
            originalCamPos = playerCamera.transform.localPosition;

        if (hazeOverlay != null)
            hazeOverlay.color = new Color(hazeOverlay.color.r, hazeOverlay.color.g, hazeOverlay.color.b, 0);
    }

    void Update()
    {
        if (dayNightCycle == null)
        {
            Debug.LogWarning("⚠️ DayNightCycle reference missing!");
            return;
        }

        bool isNight = dayNightCycle.IsNight;

        if (isNight)
        {
            if (!isInSafeZone)
                fear += increaseRate * Time.deltaTime;
            else
                fear -= decreaseRate * Time.deltaTime;
        }
        else
        {
            fear -= decreaseRate * Time.deltaTime;
        }

        fear = Mathf.Clamp(fear, 0, maxFear);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (fearSlider != null)
        {
            fearSlider.value = fear;
            if (fillImage != null)
                fillImage.color = Color.Lerp(safeColor, dangerColor, fear / maxFear);
        }

        float fearRatio = fear / maxFear;

        if (fearRatio >= 0.8f && !hasTriggeredHallucinationMessage)
        {
            hasTriggeredHallucinationMessage = true;
            if (tutorialManager != null)
            {
                tutorialManager.ShowMessage("You've been lingering outside the safe zone too long. You're now feeling an earthquake... Or are you?");
            }
        }

        // Optional: reset the message trigger if fear drops below 80%
        if (fearRatio < 0.8f)
        {
            hasTriggeredHallucinationMessage = false;
        }


        // --- Camera shake ---
        if (fearRatio >= 0.8f && playerCamera != null)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0;
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCamPos + shakeOffset, Time.deltaTime * shakeSpeed);
        }
        else if (playerCamera != null)
        {
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCamPos, Time.deltaTime * shakeSpeed);
        }

        HandleFearEffects();
    }

    void HandleFearEffects()
    {
        float fearRatio = fear / maxFear;

        // Camera shake
        if (fearRatio >= 0.8f && playerCamera != null)
        {
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0; // optional: only shake X and Y
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCamPos + shakeOffset, Time.deltaTime * shakeSpeed);
        }
        else if (playerCamera != null)
        {
            // Reset camera
            playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCamPos, Time.deltaTime * shakeSpeed);
        }

        // Haze overlay
        if (hazeOverlay != null)
        {
            float targetAlpha = Mathf.InverseLerp(0.8f, 1f, fearRatio) * hazeMaxAlpha;
            Color c = hazeOverlay.color;
            c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * 2f);
            hazeOverlay.color = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = true;
            Debug.Log("🟢 Entered Safe Zone");

            // Hide hallucination message when player is safe
            if (tutorialManager != null)
            {
                tutorialManager.HideMessage();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = false;
            Debug.Log("🔴 Left Safe Zone");
        }
    }
}
