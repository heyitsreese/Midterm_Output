using UnityEngine;

public class SafeZoneDetector : MonoBehaviour
{
    public FearMeter fearMeter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            fearMeter.isInSafeZone = true;
            Debug.Log("🟢 Entered Safe Zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("SafeZone"))
        {
            fearMeter.isInSafeZone = false;
            Debug.Log("🔴 Left Safe Zone");
        }
    }
}
