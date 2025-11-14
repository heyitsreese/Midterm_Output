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
    public Image hazeOverlay;
    public float hazeMaxAlpha = 0.5f;

    [Header("Audio")]
    public AudioSource normalMusicSource;       // assign your regular background music
    public AudioSource fearMusicSource;         // assign your eerie background music
    public AudioSource backgroundVoicesSource;  // assign creepy voices / whispers

    [Header("Messages")]
    public TutorialManager tutorialManager;
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

        // Ensure initial music states
        if (fearMusicSource != null) fearMusicSource.Stop();
        if (backgroundVoicesSource != null) backgroundVoicesSource.Stop();
        if (normalMusicSource != null && !normalMusicSource.isPlaying) normalMusicSource.Play();
    }

    void Update()
    {
        if (dayNightCycle == null)
        {
            Debug.LogWarning("⚠️ DayNightCycle reference missing!");
            return;
        }

        bool isNight = dayNightCycle.IsNight;

        // ✅ Fixed logic: works even if player never enters Safe Zone
        if (isNight)
        {
            fear += (!isInSafeZone ? increaseRate : -decreaseRate) * Time.deltaTime;
        }
        else
        {
            fear -= decreaseRate * Time.deltaTime;
        }

        fear = Mathf.Clamp(fear, 0, maxFear);

        UpdateUI();
        HandleAudio();
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

        // --- Message when hallucinations start ---
        if (fearRatio >= 0.8f && !hasTriggeredHallucinationMessage)
        {
            hasTriggeredHallucinationMessage = true;
            tutorialManager?.ShowMessage("You've been lingering outside the safe zone too long. You're now feeling an earthquake... Or are you?");
        }
        else if (fearRatio < 0.8f && hasTriggeredHallucinationMessage)
        {
            hasTriggeredHallucinationMessage = false;
            tutorialManager?.HideMessage();
        }

        HandleFearEffects(fearRatio);
    }

    void HandleFearEffects(float fearRatio)
    {
        // Camera shake
        if (playerCamera != null)
        {
            if (fearRatio >= 0.8f)
            {
                Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
                shakeOffset.z = 0;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCamPos + shakeOffset, Time.deltaTime * shakeSpeed);
            }
            else
            {
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, originalCamPos, Time.deltaTime * shakeSpeed);
            }
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

    void HandleAudio()
    {
        float fearRatio = fear / maxFear;

        // When fear hits 50%, switch music and play background voices
        if (fearRatio >= 0.5f)
        {
            if (normalMusicSource != null && normalMusicSource.isPlaying)
                normalMusicSource.Stop();

            if (fearMusicSource != null && !fearMusicSource.isPlaying)
                fearMusicSource.Play();

            if (backgroundVoicesSource != null && !backgroundVoicesSource.isPlaying)
                backgroundVoicesSource.Play();
        }
        else
        {
            // Return to calm state
            if (fearMusicSource != null && fearMusicSource.isPlaying)
                fearMusicSource.Stop();

            if (backgroundVoicesSource != null && backgroundVoicesSource.isPlaying)
                backgroundVoicesSource.Stop();

            if (normalMusicSource != null && !normalMusicSource.isPlaying)
                normalMusicSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = true;
            Debug.Log("🟢 Entered Safe Zone");
            tutorialManager?.HideMessage();
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

    public void IncreaseFear(float amount)
    {
        fear = Mathf.Clamp(fear + amount, 0f, maxFear);
    }

}
