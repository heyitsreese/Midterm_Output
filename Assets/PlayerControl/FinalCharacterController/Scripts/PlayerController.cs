using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    [Header("Basic Movement")]
    public float runAcceleration = 0.25f;
    public float runSpeed = 4f;
    public float drag = 0.1f;

    [Header("Camera Settings")]
    public float SensH = 0.1f;
    public float SensV = 0.1f;
    public float LookLimit = 89f;

    private PlayerLocamotionInput playerLocamotionInput;
    private Vector2 cameraRotate = Vector2.zero;
    private Vector2 playerTargRotation = Vector2.zero;

    void Awake()
    {
        playerLocamotionInput = GetComponent<PlayerLocamotionInput>();
    }

    void Update()
    {
        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * playerLocamotionInput.MovementInput.x + cameraForwardXZ * playerLocamotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        // Move chracter (Call once per frame)
        characterController.Move(newVelocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        cameraRotate.x += SensH * playerLocamotionInput.LookInput.x;
        cameraRotate.y = Mathf.Clamp(cameraRotate.y - SensV * playerLocamotionInput.LookInput.y, -LookLimit, LookLimit);
        playerTargRotation.x += transform.eulerAngles.x + SensH * playerLocamotionInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerTargRotation.x, 0f);

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotate.y, cameraRotate.x, 0f);
    }
}
