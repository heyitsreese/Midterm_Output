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

        // Determine movement speed from your PlayerState, not CharacterController.velocity
        bool isMoving = (movementState == PlayerMovementState.Walking ||
                        movementState == PlayerMovementState.Running ||
                        movementState == PlayerMovementState.Sprinting) && isGrounded;

        float currentInterval = walkStepInterval;
        if (movementState == PlayerMovementState.Running)
            currentInterval = runStepInterval;
        else if (movementState == PlayerMovementState.Sprinting)
            currentInterval = sprintStepInterval;

        if (isMoving)
        {
            stepTimer += Time.deltaTime;

            if (stepTimer >= currentInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            // Instead of resetting to 0, just stop incrementing until player moves again
            // stepTimer = 0f;  // remove this line
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
