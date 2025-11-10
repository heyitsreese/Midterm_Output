using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class FootStepAudio : MonoBehaviour
{
    [Header("References")]
    public AudioSource footstepSource;
    public AudioClip footstepClip;
    private PlayerState playerState;
    private CharacterController characterController;

    [Header("Footstep Settings")]
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.4f;
    public float sprintStepInterval = 0.3f;
    public float minMoveSpeed = 0.1f; // prevents idle footsteps

    private float stepTimer;

    void Start()
    {
        // Auto-assign references if not set
        footstepSource ??= GetComponent<AudioSource>();
        playerState ??= GetComponent<PlayerState>();
        characterController ??= GetComponent<CharacterController>();
    }

    void Update()
    {
        if (playerState == null || characterController == null || footstepClip == null)
            return;

        bool isGrounded = characterController.isGrounded;
        PlayerMovementState movementState = playerState.CurrentPlayerMovementState;

        // Get horizontal velocity (ignore vertical)
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        float speed = horizontalVelocity.magnitude;

        bool isMoving = speed > minMoveSpeed && isGrounded &&
                        (movementState == PlayerMovementState.Walking ||
                         movementState == PlayerMovementState.Running ||
                         movementState == PlayerMovementState.Sprinting);

        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            float currentInterval = walkStepInterval;
            if (movementState == PlayerMovementState.Running)
                currentInterval = runStepInterval;
            else if (movementState == PlayerMovementState.Sprinting)
                currentInterval = sprintStepInterval;

            if (stepTimer >= currentInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void PlayFootstep()
    {
        if (!footstepSource.enabled)
            footstepSource.enabled = true;

        footstepSource.pitch = Random.Range(0.95f, 1.05f);
        footstepSource.PlayOneShot(footstepClip);
    }
}
