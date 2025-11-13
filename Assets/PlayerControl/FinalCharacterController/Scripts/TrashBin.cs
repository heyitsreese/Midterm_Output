using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TrashType acceptedType;
    public AudioSource wrongBinSound; // assign this in Inspector
    public GhostManager ghostManager;

    private void OnTriggerEnter(Collider other)
    {
        TrashItem trashItem = other.GetComponent<TrashItem>();
        if (trashItem == null) return;

        if (trashItem.trashType != acceptedType)
        {
            Debug.Log("❌ Wrong bin! The world grows more haunted...");
            
            wrongBinSound.Play();

            GhostManager.Instance.ActivateGhostAggression();
        }
        else
        {
            Debug.Log("✅ Correct bin!");
        }

        Destroy(other.gameObject);
    }
}
