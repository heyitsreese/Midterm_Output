// using UnityEngine;

// public class EnemyFollow : MonoBehaviour
// {
//     public Transform player;         
//     public float moveSpeed = 3f;      
//     public float followStartRadius = 5f;  
//     //public float followStopRadius = 10f;   
//     public float rotationSpeed = 5f;     

//     private Vector3 originalPosition;
//     private bool isFollowing = false;      
    
//     // if the player has the relic to make ghost go away?
//     private bool hasRelic = false;

//     void Start()
//     {
//         originalPosition = transform.position;
//     }

//     void Update()
//     {
//         if (player != null)
//         {
//             float distance = Vector3.Distance(transform.position, player.position);

//             if (distance <= followStartRadius)
//             {
//                 // Start following the player
//                 isFollowing = true;
//             }

//             // enemy stops chasing if player gets far enough 
//             // we can just use this if we cant add the relic thing 
//             // if (distance > followStopRadius && isFollowing)
//             // {
//             //    
//             //     isFollowing = false;
//             // }

//             // if player holds relic the monster will stop following ?
//             if (hasRelic && isFollowing)
//             {
//                 isFollowing = false;
//             }

//             if (isFollowing)
//             {
//                 // add code that adds to fear meter while following each second 
//                 Vector3 direction = (player.position - transform.position).normalized;

//                 transform.position += direction * moveSpeed * Time.deltaTime;

//                 Quaternion lookRotation = Quaternion.LookRotation(direction);
//                 transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
//             }
//             else
//             {
//                 transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);

                
//                 if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
//                 {
//                     transform.position = originalPosition; 
//                 }
//             }
//         }
//     }
// }

using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float followStartRadius = 5f;
    public float stopFollowDistance = 15f;

    [HideInInspector] public bool isAggressive = false;

    [Header("Fear Integration")]
    public float fearIncreaseRate = 5f;

    private Vector3 originalPosition;
    private SafeZoneManager safeZoneManager;
    private DayNightCycle dayNightCycle;
    private FearMeter fearMeter;

    void Start()
    {
        originalPosition = transform.position;
        safeZoneManager = FindObjectOfType<SafeZoneManager>();
        dayNightCycle = FindObjectOfType<DayNightCycle>();
        
        fearMeter = player != null ? player.GetComponent<FearMeter>() : null;
    }

    void Update()
    {
        if (player == null && GhostManager.Instance != null)
        {
            player = GhostManager.Instance.player.transform;
            if (player != null && fearMeter == null)
                fearMeter = player.GetComponent<FearMeter>();
        }

        if (player == null) return; // Still null? Then skip this frame

        bool isNight = dayNightCycle != null && dayNightCycle.IsNight;

        // Chase only if night or temporarily aggressive
        if (isNight || isAggressive)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Stop chasing if far away
            if (isAggressive && distance > stopFollowDistance)
            {
                ResetChase();
                return;
            }

            if (distance <= followStartRadius || isAggressive)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

                // Increase fear if aggressive
                if (isAggressive && fearMeter != null)
                    fearMeter.IncreaseFear(fearIncreaseRate * Time.deltaTime);
            }
            else
            {
                ReturnToOrigin();
            }
        }
        else
        {
            ReturnToOrigin();
        }
    }

    void ResetChase()
    {
        isAggressive = false;
        ReturnToOrigin();
    }

    void ReturnToOrigin()
    {
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
            transform.position = originalPosition;
    }
}
