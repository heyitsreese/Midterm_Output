using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TrashType acceptedType;
    //public AudioSource source; // assign this in Inspector
    public GhostManager ghostManager;

    // use part of enemy follow script here 
    // add count for how many times the player puts trash in wrong bin 

    void Awake()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        TrashItem trashItem = other.GetComponent<TrashItem>();
        //source = GetComponent<AudioSource>();

        if (trashItem == null) return;

        if (trashItem.trashType != acceptedType)
        {
            //Debug.Log("❌ Wrong bin! The world grows more haunted...");
            Debug.Log("TEST");
            
            //source.Play();

            GhostManager.Instance.ActivateGhostAggression();

            // from ghost script the ghost will chase the player 
        }
        else
        {
            // add sound effect 
            Debug.Log("✅ Correct bin!");
        }

        //Destroy(other.gameObject);
    }
}
