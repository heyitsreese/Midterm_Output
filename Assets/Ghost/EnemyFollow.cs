using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public Transform player;         
    public float moveSpeed = 3f;      
    public float followStartRadius = 5f;  
    //public float followStopRadius = 10f;   
    public float rotationSpeed = 5f;     

    private Vector3 originalPosition;
    private bool isFollowing = false;      
    
    // if the player has the relic to make ghost go away?
    private bool hasRelic = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= followStartRadius)
            {
                // Start following the player
                isFollowing = true;
            }

            // enemy stops chasing if player gets far enough 
            // we can just use this if we cant add the relic thing 
            // if (distance > followStopRadius && isFollowing)
            // {
            //    
            //     isFollowing = false;
            // }

            // if player holds relic the monster will stop following ?
            if (hasRelic && isFollowing)
            {
                isFollowing = false;
            }

            if (isFollowing)
            {
                // add code that adds to fear meter while following each second 
                Vector3 direction = (player.position - transform.position).normalized;

                transform.position += direction * moveSpeed * Time.deltaTime;

                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);

                
                if (Vector3.Distance(transform.position, originalPosition) < 0.1f)
                {
                    transform.position = originalPosition; 
                }
            }
        }
    }
}

