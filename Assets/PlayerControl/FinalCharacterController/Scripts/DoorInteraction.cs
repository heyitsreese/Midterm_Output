using UnityEngine;
using UnityEngine.InputSystem;

public class DoorInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Maximum distance for interacting with the door")]
    public float interactionRange = 3.5f;

    private DayNightCycle dayNightCycle;
    private Transform player;

    void Start()
    {
        // Find and cache the DayNightCycle component
        dayNightCycle = FindFirstObjectByType<DayNightCycle>();

        // Find the player by tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("⚠️ Player not found! Make sure your Player GameObject is tagged 'Player'.");
        }

        // Warn if DayNightCycle is missing
        if (dayNightCycle == null)
        {
            Debug.LogWarning("⚠️ DayNightCycle not found in scene!");
        }
    }

    void Update()
    {
        if (player == null || dayNightCycle == null || Mouse.current == null)
            return;

        // Detect right-click
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            float distance = Vector3.Distance(player.position, transform.position);

            // Too far
            if (distance > interactionRange)
            {
                Debug.Log("🚶 You're too far from the door to interact.");
                return;
            }

            // Has the player picked up the key?
            if (PlayerInventory.hasKey)
            {
                Debug.Log("🔓 Door unlocked! You win!");
                dayNightCycle.WinGame();
            }
            else
            {
                Debug.Log("🚫 The door is locked. You need a key.");
            }
        }
    }

    // Optional visualization in Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
