using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TrashType acceptedType;
    public AudioSource wrongBinSound; // assign this in Inspector

    private void OnTriggerEnter(Collider other)
    {
        TrashItem trashItem = other.GetComponent<TrashItem>();
        if (trashItem == null) return;

        if (trashItem.trashType != acceptedType)
        {
            Debug.Log("❌ Wrong bin! The world grows more haunted...");

            // Play wrong bin whisper
            if (wrongBinSound != null)
            {
                wrongBinSound.Stop(); // ensures it restarts if already playing
                wrongBinSound.Play();
            }

            GhostManager.Instance?.AddCorruption();
        }
        else
        {
            Debug.Log("✅ Correct bin!");
        }

        Destroy(other.gameObject);
    }
}
