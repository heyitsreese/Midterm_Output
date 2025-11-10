using UnityEngine;
using UnityEngine.UI;

public class FearMeter : MonoBehaviour
{
    [Header("Fear Settings")]
    public float fear = 0f;
    public float maxFear = 100f;
    public float increaseRate = 5f;   // Fear rises faster at night outside safe zone
    public float decreaseRate = 2f;   // Fear slowly drops in safe zone or daytime
    public bool isInSafeZone = true;

    [Header("Day-Night Reference")]
    public DayNightCycle dayNightCycle; // Drag your DayNightCycle GameObject here

    [Header("UI")]
    public Slider fearSlider;
    public Image fillImage;
    public Color safeColor = Color.green;
    public Color dangerColor = Color.red;

    void Start()
    {
        if (fearSlider != null)
        {
            fearSlider.maxValue = maxFear;
            fearSlider.value = fear;
        }
    }

    void Update()
    {
        if (dayNightCycle == null)
        {
            Debug.LogWarning("⚠️ DayNightCycle reference missing!");
            return;
        }

        bool isNight = dayNightCycle.IsNight; // ✅ Uses your public getter

        if (!isInSafeZone && isNight)
            fear += increaseRate * Time.deltaTime;
        else
            fear -= decreaseRate * Time.deltaTime;

        fear = Mathf.Clamp(fear, 0, maxFear);

        if (fearSlider != null)
        {
            fearSlider.value = fear;
            if (fillImage != null)
                fillImage.color = Color.Lerp(safeColor, dangerColor, fear / maxFear);
        }

        if (fear >= maxFear)
        {
            Debug.Log("😱 Fear maxed out! The player is in danger!");
            // Optional: trigger death, fainting, or hallucination
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            isInSafeZone = true;
            Debug.Log("🟢 Entered Safe Zone");
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
