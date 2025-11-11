using UnityEngine;

public class SafeZoneManager : MonoBehaviour
{
    [Header("Safe Zone Settings")]
    public Transform centerPoint; // e.g. center of your terrain
    public float safeRadius = 50f; // size of your safe zone

    [Header("Player Reference")]
    public Transform player;

    private bool isPlayerSafe;

    void Update()
    {
        if (player == null || centerPoint == null) return;

        float distance = Vector3.Distance(player.position, centerPoint.position);

        bool currentlySafe = distance <= safeRadius;

        if (currentlySafe != isPlayerSafe)
        {
            isPlayerSafe = currentlySafe;
            if (isPlayerSafe)
                OnEnterSafeZone();
            else
                OnExitSafeZone();
        }
    }

    public bool IsPlayerSafe()
    {
        return isPlayerSafe;
    }

    void OnDrawGizmosSelected()
    {
        if (centerPoint == null) return;

        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawSphere(centerPoint.position, safeRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(centerPoint.position, safeRadius);
    }

    void OnEnterSafeZone()
    {
        Debug.Log("🟢 Player entered SAFE ZONE!");
        // Example: disable enemy attacks, heal player, stop ambient danger sounds
    }

    void OnExitSafeZone()
    {
        Debug.Log("🔴 Player entered UNSAFE ZONE!");
        // Example: enable enemy spawns, start ambient danger sounds, etc.
    }
}
