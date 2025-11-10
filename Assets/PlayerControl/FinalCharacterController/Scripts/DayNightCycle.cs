// using UnityEngine;
// using TMPro;

// public class DayNightCycle : MonoBehaviour
// {
//     [Header("Cycle Settings")]
//     public Light directionalLight;
//     public int totalDays = 5;          // Game ends after Day 5
//     public float dayDuration = 60f;    // 60 seconds day
//     public float nightDuration = 60f;  // 60 seconds night

//     [Header("Door Settings")]
//     public GameObject doorMesh;
//     public Vector3 doorSpawnPosition;

//     [Header("UI")]
//     public TMP_Text dayCounterText;    // Displays "Day X"

//     private int currentDay = 1;
//     private float cycleTimer = 0f;
//     private bool isNight = false;
//     private bool gameEnded = false;

//     void Start()
//     {
//         if (doorMesh != null)
//             doorMesh.SetActive(false);

//         UpdateDayCounterUI();
//     }

//     void Update()
//     {
//         if (gameEnded)
//             return; // ✅ Stop everything after Day 5

//         cycleTimer += Time.deltaTime;

//         if (!isNight)
//         {
//             // Daytime rotation: 0° → 180°
//             float progress = cycleTimer / dayDuration;
//             float rotation = Mathf.Lerp(0f, 180f, progress);
//             directionalLight.transform.rotation = Quaternion.Euler(rotation, 0, 0);

//             if (cycleTimer >= dayDuration)
//             {
//                 isNight = true;
//                 cycleTimer = 0f;
//                 Debug.Log($"🌙 Night has begun (Day {currentDay})");
//             }
//         }
//         else
//         {
//             // Nighttime rotation: 180° → 360°
//             float progress = cycleTimer / nightDuration;
//             float rotation = Mathf.Lerp(180f, 360f, progress);
//             directionalLight.transform.rotation = Quaternion.Euler(rotation, 0, 0);

//             if (cycleTimer >= nightDuration)
//             {
//                 isNight = false;
//                 cycleTimer = 0f;
//                 currentDay++;
//                 UpdateDayCounterUI();

//                 Debug.Log($"☀️ Day {currentDay} started!");

//                 if (currentDay == totalDays)
//                 {
//                     SpawnDoor();
//                 }
//                 else if (currentDay > totalDays)
//                 {
//                     EndGame();
//                 }
//             }
//         }
//     }

//     void UpdateDayCounterUI()
//     {
//         if (dayCounterText != null)
//             dayCounterText.text = "Day " + currentDay;
//     }

//     void SpawnDoor()
//     {
//         if (doorMesh == null) return;

//         doorMesh.transform.position = doorSpawnPosition;
//         doorMesh.SetActive(true);
//         Debug.Log($"🚪 Door appeared on Day {currentDay}!");
//     }

//     void EndGame()
//     {
//         gameEnded = true;
//         Debug.Log("🕒 Day 5 completed! Checking win/loss conditions...");
        
//         // Optional: Freeze time or trigger results screen
//         // Time.timeScale = 0f;

//         // You could also call a GameManager to handle win/loss here
//     }
// }

using UnityEngine;
using TMPro;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public Light directionalLight;
    public int totalDays = 5;
    public float dayDuration = 60f;
    public float nightDuration = 60f;

    [Header("Objects")]
    public GameObject doorMesh;
    public GameObject keyMesh;
    public WinLoss winLoss;
    public Vector3 doorSpawnPosition;
    public Vector3 keySpawnPosition;

    [Header("UI")]
    public TMP_Text dayCounterText;

    private int currentDay = 1;
    public int CurrentDay => currentDay;
    private float cycleTimer = 0f;
    private bool isNight = false;
    public bool IsNight => isNight;
    private bool gameEnded = false;

    void Start()
    {
        if (doorMesh != null) doorMesh.SetActive(false);
        if (keyMesh != null && currentDay < 4)
            keyMesh.SetActive(false);

        UpdateDayCounterUI();
    }

    void Update()
    {
        if (gameEnded)
            return;

        cycleTimer += Time.deltaTime;

        if (!isNight)
        {
            float progress = cycleTimer / dayDuration;
            float rotation = Mathf.Lerp(0f, 180f, progress);
            directionalLight.transform.rotation = Quaternion.Euler(rotation, 0, 0);

            if (cycleTimer >= dayDuration)
            {
                isNight = true;
                cycleTimer = 0f;
                Debug.Log($"🌙 Night has begun (Day {currentDay})");
            }
        }
        else
        {
            float progress = cycleTimer / nightDuration;
            float rotation = Mathf.Lerp(180f, 360f, progress);
            directionalLight.transform.rotation = Quaternion.Euler(rotation, 0, 0);

            if (cycleTimer >= nightDuration)
            {
                isNight = false;
                cycleTimer = 0f;
                currentDay++;
                UpdateDayCounterUI();
                Debug.Log($"☀️ Day {currentDay} started!");

                if (currentDay == 4)
                    SpawnKey();
                else if (currentDay == 5)
                    SpawnDoor();
                else if (currentDay > totalDays)
                    EndGame();
            }
        }
        if (keyMesh != null && keyMesh.activeSelf == false)
        {
            Debug.LogWarning("⚠️ Key was deactivated unexpectedly!");
        }
    }

    void UpdateDayCounterUI()
    {
        if (dayCounterText != null)
            dayCounterText.text = "Day " + currentDay;
    }

    void SpawnKey()
    {
        if (keyMesh == null)
        {
            Debug.LogError("❌ KeyMesh not assigned in DayNightCycle!");
            return;
        }

        Debug.Log($"🗝️ Trying to spawn key at {keySpawnPosition}");

        keyMesh.transform.position = keySpawnPosition;
        keyMesh.SetActive(true);

        Debug.Log($"✅ Key active? {keyMesh.activeSelf} | Position: {keyMesh.transform.position}");
    }

    public void SpawnDoor()
    {
        if (doorMesh == null || doorMesh.activeSelf) return;

        doorMesh.transform.position = doorSpawnPosition;
        doorMesh.SetActive(true);
        Debug.Log("🚪 Door appeared on Day 5!");
    }

    public void WinGame()
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f;
        Debug.Log("🎉 You unlocked the door and won the game!");

        if (winLoss != null)
            winLoss.ShowWinScreen();
        else
            Debug.LogWarning("⚠️ WinLoss not assigned in DayNightCycle!");
    }

    void EndGame()
    {
        gameEnded = true;
        Debug.Log("🕒 Day 5 ended! Game over — you didn’t unlock the door in time.");

        if (winLoss != null)
            winLoss.ShowLossScreen();
        else
            Debug.LogWarning("⚠️ WinLoss not assigned in DayNightCycle!");
    }

}
