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
    public bool isDaytime = true;

    [HideInInspector] public bool isAggressive = false;

    private Rigidbody rb;

    // wandering
    private Vector3 wanderDirection;
    private float wanderChangeInterval = 3f;
    private float wanderTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (player == null && GhostManager.Instance != null && GhostManager.Instance.player != null)
            player = GhostManager.Instance.player.transform;

        PickNewWanderDirection();
    }

    void FixedUpdate()
    {
        if (isAggressive && player != null)
        {
            ChasePlayer();
        }
        else
        {
            WanderAround();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // keep movement horizontal

        float distance = direction.magnitude;

        if (distance < stopFollowDistance)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            MoveWithObstacleAvoidance();
        }
    }

    void WanderAround()
    {
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderChangeInterval)
        {
            PickNewWanderDirection();
            wanderTimer = 0f;
        }

        MoveWithObstacleAvoidance();
    }

    void PickNewWanderDirection()
    {
        // Choose a random horizontal direction
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        wanderDirection = new Vector3(randomDir.x, 0, randomDir.y);
    }

    void MoveWithObstacleAvoidance()
    {
        Vector3 forward = isAggressive && player != null
            ? (player.position - transform.position).normalized
            : wanderDirection;

        // Smooth rotation
        Quaternion targetRotation = Quaternion.LookRotation(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Raycast forward to detect obstacles
        if (!Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, 1f))
        {
            rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.deltaTime);
        }
        else
        {
            // if blocked, try turning slightly
            transform.Rotate(0, Random.Range(-90f, 90f), 0);
            PickNewWanderDirection();
        }
    }
}
