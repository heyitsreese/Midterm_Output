// using System;
// using UnityEngine;
// using UnityEngine.InputSystem;

// public class PickupObject : MonoBehaviour
// {
//     [Header("Pickup Settings")]
//     public float radius = 2f;
//     public float distance = 2f;
//     public float height = 1.6f;
//     public float smoothSpeed = 10f;
//     public float throwRange = 3f; // how far the ray can detect a bin

//     private GameObject holdObject;
//     private Rigidbody holdRigidbody;
//     private Collider holdCollider;
//     private TutorialManager tutorialManager;

//     void Start()
//     {
//         tutorialManager = FindFirstObjectByType<TutorialManager>();

//         if (tutorialManager == null)
//             Debug.LogWarning("⚠️ TutorialManager not found in scene.");
//     }

//     void Update()
//     {
//         var keyboard = Keyboard.current;
//         var mouse = Mouse.current;
//         if (keyboard == null || mouse == null) return;

//         bool pressedE = keyboard.eKey.wasPressedThisFrame;
//         bool clickedLeft = mouse.leftButton.wasPressedThisFrame;
//         Transform t = transform;

//         if (holdObject && pressedE)
//         {
//             DropObject();
//         }
//         else if (!holdObject && pressedE)
//         {
//             TryPickupObject(t);
//         }

//         if (holdObject && clickedLeft)
//         {
//             TryThrowIntoBin(t);
//         }
//     }

//     void TryPickupObject(Transform t)
//     {
//         Collider[] hits = Physics.OverlapSphere(t.position + t.forward * 1f, radius);

//         foreach (var hit in hits)
//         {
//             if (hit.CompareTag("Pickupable"))
//             {
//                 holdObject = hit.gameObject;
//                 holdRigidbody = holdObject.GetComponent<Rigidbody>();
//                     if (holdRigidbody == null)
//                         holdRigidbody = holdObject.GetComponentInChildren<Rigidbody>();
//                 holdCollider = holdObject.GetComponent<Collider>();
//                     if (holdCollider == null)
//                         holdCollider = holdObject.GetComponentInChildren<Collider>();

//                 if (holdRigidbody != null)
//                 {
//                     holdObject.transform.position += Vector3.up * 0.2f;
//                     holdRigidbody.isKinematic = true;
//                     holdRigidbody.useGravity = false;
//                     holdRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
// #if UNITY_6000_0_OR_NEWER
//                     holdRigidbody.linearVelocity = Vector3.zero;
// #else
//                     holdRigidbody.velocity = Vector3.zero;
// #endif
//                 }

//                 if (holdCollider != null)
//                     holdCollider.enabled = false;

//                 if (tutorialManager != null)
//                     tutorialManager.NotifyItemPickedUp();

//                 Debug.Log($"🟢 Picked up {holdObject.name}");
//                 return;
//             }
//         }

//         Debug.Log("❌ No pickupable object found nearby.");
//     }

//     void TryThrowIntoBin(Transform t)
//     {
//         if (holdObject == null) return;

//         // Use raycast from the camera or player's eyes
//         Ray ray = new Ray(t.position + Vector3.up * 1.2f, t.forward);

//         // Increase the range slightly for reliability
//         if (Physics.Raycast(ray, out RaycastHit hit, throwRange))
//         {
//             TrashBin bin = hit.collider.GetComponentInParent<TrashBin>();
//             TrashItem item = holdObject.GetComponent<TrashItem>();

//             Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);

//             if (bin != null && item != null)
//             {
//                 if (bin.acceptedType == item.trashType)
//                 {
//                     Debug.Log($"✅ Correctly disposed {item.trashType} waste in {bin.name}!");

//                     // "Throw" effect — simply destroy the held object
//                     Destroy(holdObject);
//                     holdObject = null;
//                     holdRigidbody = null;
//                     holdCollider = null;
//                 }
//                 else
//                 {
//                     Debug.Log($"❌ Wrong bin! That belongs in {item.trashType} waste, not {bin.acceptedType}.");
//                 }
//             }
//             else
//             {
//                 Debug.Log($"⚠️ You hit {hit.collider.name}, but it’s not a valid bin.");
//             }
//         }
//         else
//         {
//             Debug.DrawRay(ray.origin, ray.direction * throwRange, Color.red, 2f);
//             Debug.Log("📏 No bin detected in front.");
//         }
//     }

//     void DropObject()
//     {
//         if (holdRigidbody != null)
//         {
//             holdRigidbody.isKinematic = false;
//             holdRigidbody.useGravity = true;
//             holdRigidbody.constraints = RigidbodyConstraints.None;
//         }

//         if (holdCollider != null)
//             holdCollider.enabled = true;

//         Debug.Log($"🔴 Dropped {holdObject.name}");
//         holdObject = null;
//         holdRigidbody = null;
//         holdCollider = null;
//     }

//     void FixedUpdate()
//     {
//         if (holdObject && holdRigidbody)
//         {
//             var t = transform;
//             Vector3 targetPos = t.position + t.forward * distance + t.up * height;
//             Vector3 newPos = Vector3.Lerp(holdObject.transform.position, targetPos, Time.fixedDeltaTime * smoothSpeed);

//             holdObject.transform.position = newPos;
//             holdObject.transform.rotation = Quaternion.Lerp(holdObject.transform.rotation, t.rotation, Time.fixedDeltaTime * smoothSpeed);
//         }
//     }

//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, radius);
//     }
// }

using UnityEngine;
using UnityEngine.InputSystem;

public class PickupObject : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float radius = 2f;
    public float distance = 2f;
    public float height = 1.6f;
    public float smoothSpeed = 10f;
    public float throwRange = 3f; // how far the ray can detect a bin

    private GameObject holdObject;
    private Rigidbody holdRigidbody;
    private Collider holdCollider;
    private TutorialManager tutorialManager;

    void Start()
    {
        tutorialManager = FindFirstObjectByType<TutorialManager>();
        if (tutorialManager == null)
            Debug.LogWarning("⚠️ TutorialManager not found in scene.");
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        bool pressedE = keyboard.eKey.wasPressedThisFrame;
        bool clickedLeft = mouse.leftButton.wasPressedThisFrame;

        if (holdObject && pressedE)
        {
            DropObject();
        }
        else if (!holdObject && pressedE)
        {
            TryPickupObject();
        }

        if (holdObject && clickedLeft)
        {
            TryThrowIntoBin();
        }
    }

    void TryPickupObject()
    {
        Transform t = transform;
        Collider[] hits = Physics.OverlapSphere(t.position + t.forward * 1f, radius);

        foreach (var hit in hits)
        {
            // Only pick objects tagged as Pickupable
            GameObject target = hit.CompareTag("Pickupable") ? hit.gameObject : hit.transform.root.gameObject;
            if (!target.CompareTag("Pickupable")) continue;

            holdObject = target;

            // Get Rigidbody from self or children
            holdRigidbody = holdObject.GetComponent<Rigidbody>() ?? holdObject.GetComponentInChildren<Rigidbody>();
            if (holdRigidbody != null)
            {
                holdRigidbody.isKinematic = true;
                holdRigidbody.useGravity = false;
                holdRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
#if UNITY_6000_0_OR_NEWER
                holdRigidbody.linearVelocity = Vector3.zero;
#else
                holdRigidbody.velocity = Vector3.zero;
#endif
            }

            // Get Collider from self or children
            holdCollider = holdObject.GetComponent<Collider>() ?? holdObject.GetComponentInChildren<Collider>();
            if (holdCollider != null)
                holdCollider.enabled = false;

            // Slightly lift object
            holdObject.transform.position += Vector3.up * 0.2f;

            // Notify tutorial manager
            tutorialManager?.NotifyItemPickedUp();

            Debug.Log($"🟢 Picked up {holdObject.name}");
            return;
        }

        Debug.Log("❌ No pickupable object found nearby.");
    }

    void TryThrowIntoBin()
    {
        if (holdObject == null) return;

        Transform t = transform;
        Ray ray = new Ray(t.position + Vector3.up * 1.2f, t.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, throwRange))
        {
            TrashBin bin = hit.collider.GetComponentInParent<TrashBin>();
            TrashItem item = holdObject.GetComponent<TrashItem>();

            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green, 2f);

            if (bin != null && item != null)
            {
                if (bin.acceptedType == item.trashType)
                {
                    Debug.Log($"✅ Correctly disposed {item.trashType} waste in {bin.name}!");
                    Destroy(holdObject);
                    holdObject = null;
                    holdRigidbody = null;
                    holdCollider = null;
                }
                else
                {
                    Debug.Log($"❌ Wrong bin! That belongs in {item.trashType} waste, not {bin.acceptedType}.");
                }
            }
            else
            {
                Debug.Log($"⚠️ You hit {hit.collider.name}, but it’s not a valid bin.");
            }
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * throwRange, Color.red, 2f);
            Debug.Log("📏 No bin detected in front.");
        }
    }

    void DropObject()
    {
        if (holdRigidbody != null)
        {
            holdRigidbody.isKinematic = false;
            holdRigidbody.useGravity = true;
            holdRigidbody.constraints = RigidbodyConstraints.None;
        }

        if (holdCollider != null)
            holdCollider.enabled = true;

        Debug.Log($"🔴 Dropped {holdObject.name}");
        holdObject = null;
        holdRigidbody = null; 
        holdCollider = null;
    }

    void FixedUpdate()
    {
        if (holdObject && holdRigidbody)
        {
            Transform t = transform;
            Vector3 targetPos = t.position + t.forward * distance + t.up * height;
            holdObject.transform.position = Vector3.Lerp(holdObject.transform.position, targetPos, Time.fixedDeltaTime * smoothSpeed);
            holdObject.transform.rotation = Quaternion.Lerp(holdObject.transform.rotation, t.rotation, Time.fixedDeltaTime * smoothSpeed);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, radius);
    }
}
