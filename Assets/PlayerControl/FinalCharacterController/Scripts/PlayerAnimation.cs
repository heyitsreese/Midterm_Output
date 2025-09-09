using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float locamotionBlendSpeed = 0.02f;

    private PlayerLocamotionInput playerLocamotionInput;
    private PlayerState playerState;

    private static int inputXHash = Animator.StringToHash("InputX");
    private static int inputYHash = Animator.StringToHash("InputY");
    private static int InputMagHash = Animator.StringToHash("InputMag");
    private static int IsGroundedMesh = Animator.StringToHash("IsGrounded");
    private static int IsFallingMesh = Animator.StringToHash("IsFalling");
    private static int IsJumpingHash = Animator.StringToHash("IsJumping");

    private Vector3 currentBlendInput = Vector3.zero;

    private void Awake()
    {
        playerLocamotionInput = GetComponent<PlayerLocamotionInput>();
        playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isIdling = playerState.CurrentPlayerMovementState == PlayerMovementState.Idling;
        bool isRunning = playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
        bool isGrounded = playerState.IsGroundedState();

        Vector2 inputTarget = isSprinting ? playerLocamotionInput.MovementInput * 1.5f : playerLocamotionInput.MovementInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locamotionBlendSpeed * Time.deltaTime);

        animator.SetBool(IsGroundedMesh, isGrounded);
        animator.SetBool(IsFallingMesh, isFalling);
        animator.SetBool(IsJumpingHash, isJumping);

        animator.SetFloat(inputXHash, currentBlendInput.x);
        animator.SetFloat(inputYHash, currentBlendInput.y);
        animator.SetFloat(InputMagHash, currentBlendInput.magnitude);
    }
}
