using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("References")]
    public GameObject player; // assign Player here
    public AudioClip pickupSfx;
    private AudioSource audioSource;

    [Header("Pickup Settings")]
    public float pickupRange = 3f;

    private bool isCollected = false;

    void Awake()
    {
        if (pickupSfx != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = pickupSfx;
        }
    }

    void Update()
    {
        if (isCollected || player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= pickupRange)
        {
            Debug.Log($"[KeyPickup] Player in range ({distance:F2}m). Press E to pick up the key.");

            if (Input.GetKeyDown(KeyCode.E))
            {
                CollectKey();
            }
        }
    }

    void CollectKey()
    {
        isCollected = true;

        PlayerInventory.hasKey = true; // just set it directly!
        Debug.Log("✅ PlayerInventory.hasKey = true");

        if (audioSource != null)
            audioSource.Play();

        Debug.Log("[KeyPickup] Key collected!");
        gameObject.SetActive(false);
    }
}
