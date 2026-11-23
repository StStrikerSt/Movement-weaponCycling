using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody Rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    [SerializeField] private PlayerProfile profile;
    [SerializeField] private PlayerTraversalState currentState = PlayerTraversalState.Idle;

    [SerializeField] private Vector3 currentHorizontalVelocity;
    [SerializeField] private float verticalVelocity;
    [SerializeField] private FollowCamera followCamera;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float jumpForce = 7f;
[SerializeField] private float gravity = -20f;
[SerializeField] private float sprintSpeed = 8f;

[SerializeField] private Transform groundCheck;
[SerializeField] private float groundCheckRadius = 0.2f;
[SerializeField] private LayerMask groundLayer;

private bool isGrounded;
private bool isSprinting;
private bool jumpRequested;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        //hides the mouse cursor
        Cursor.visible = false;
    }

    private void FixedUpdate()
    {
        if (profile == null || Rb == null) return;
        UpdateState();
        HandleMovement();
        ApplyFinalVelocity();
        
    }

    private void LateUpdate()
    {
        //checks for referenced camera
        if (followCamera != null)
        {
            //sets the look input 
            followCamera.SetLookInput(lookInput);
        }
    }

    private void UpdateState()
    {
        Debug.Log(moveInput);
        //checks for movement input
        if (moveInput.sqrMagnitude > 0.01f)
        {
            //switches state machine to walk
            currentState = PlayerTraversalState.Walk;
        }
        else
        {
            //switches state machine to idle
            currentState = PlayerTraversalState.Idle;
        }
    }
    private void HandleMovement()
    {
        switch (currentState)
        {
            case PlayerTraversalState.Idle:
                HandleIdle();
                break;

            case PlayerTraversalState.Walk:
                HandleWalk();
                break;
        }
    }

    private void HandleIdle()
    {
        //sets horizontal velocity to zero if no input detected after the deceleration rate has concluded
        currentHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            Vector3.zero,
            profile.groundDeceleration * Time.fixedDeltaTime);

    }

    private void HandleWalk()
    {
        //sets the moevement direction based on where the camera is facing
        Vector3 moveDirection = GetCameraRelativeMoveDirection();
        //set the velocity the player should be at based on the direction they are moving by their default values
        Vector3 targetVelocity = moveDirection * profile.walkSpeed;
        //sets the current velocity to target velocity after the accelaration rate
        currentHorizontalVelocity = Vector3.MoveTowards(
            currentHorizontalVelocity,
            targetVelocity,
            profile.groundAcceleration * Time.fixedDeltaTime
        );
        //checks for input
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            transform.forward = moveDirection;
        }
    }

    private Vector3 GetCameraRelativeMoveDirection()
    {
        if (cameraTransform == null)
        {
            return new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        }

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * moveInput.y) + (cameraRight * moveInput.x);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        return moveDirection;
    }
    private void ApplyFinalVelocity()
    {
        //sets final veloctiy equal to their currentvelocity
        Vector3 finalVelocity = currentHorizontalVelocity;
        finalVelocity.y = verticalVelocity;

        Rb.linearVelocity = finalVelocity;
    }
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("MoveCalled" + moveInput);
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}
