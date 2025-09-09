using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerLocamotionInput : MonoBehaviour, PlayerControls.IPlayerLocamotionMapActions
{

    public PlayerControls PlayerControls { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }


    private void OnEnable()
    {
        PlayerControls = new PlayerControls();
        PlayerControls.Enable();

        PlayerControls.PlayerLocamotionMap.Enable();
        PlayerControls.PlayerLocamotionMap.SetCallbacks(this);
    }

    void OnDisable()
    {
        PlayerControls.PlayerLocamotionMap.Disable();
        PlayerControls.PlayerLocamotionMap.RemoveCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        print(MovementInput);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }
}
