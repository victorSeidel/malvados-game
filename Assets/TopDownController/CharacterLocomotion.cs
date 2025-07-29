using UnityEngine;

public class CharacterLocomotion : MonoBehaviour
{
    [Tooltip("Character controller is a built in component in unity. Feel free to use rigidbody or changing transform directly")]
    [SerializeField] CharacterController characterController;
    [Tooltip("how fast the player walks")]
    [SerializeField] float walkSpeed = 3f;
    [Tooltip("if you would like separate visual from player assign something else here")]
    [SerializeField] Transform characterVisual;

    [Tooltip("Use isso se o modelo estiver virado errado (em graus, por exemplo -90)")]
    [SerializeField] float visualRotationOffset = -90f;

    [Tooltip("Feel free to assign other joysticks here")]
    [SerializeField] FixedJoystick moveJoystick;
    [Tooltip("Self explanatory. After this magnitude player will move ")]
    [SerializeField] float movementThreshold = 0.1f;

    [Header("Animation variables")]
    [SerializeField] Animator animator;

    string currentState = "Idle";

    [SerializeField] float walkingThreshold = 0.5f;

    float mag;
    Vector3 fwd, right;

    void Awake()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        if (characterVisual == null)
        {
            characterVisual = transform;
        }
    }

    void Start()
    {
        RecalculateCamera(Camera.main);
    }

    void Update()
    {
        mag = Mathf.Clamp01(new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical).sqrMagnitude);

        if (mag >= movementThreshold)
        {
            MovementAndRotation();
        }
        else
        {
            characterController.Move(new Vector3(0, -9.8f, 0));
        }

        UpdateStateMachine();
    }

    void RecalculateCamera(Camera _cam)
    {
        Camera cam = _cam;
        fwd = cam.transform.forward;
        fwd.y = 0;
        fwd = Vector3.Normalize(fwd);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * fwd;
    }

    void MovementAndRotation()
    {
        Vector3 rightMovement = right * walkSpeed * Time.deltaTime * moveJoystick.Horizontal;
        Vector3 upMovement = fwd * walkSpeed * Time.deltaTime * moveJoystick.Vertical;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        heading.y = characterController.isGrounded ? -1f : -9.8f;

        characterController.Move(heading * walkSpeed * Time.deltaTime);

        Vector3 lookDirection = new Vector3(heading.x, 0, heading.z);
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            characterVisual.rotation = targetRotation * Quaternion.Euler(0, visualRotationOffset, 0);
        }

    }

    void UpdateStateMachine()
    {
        float joystickMagnitude = new Vector2(moveJoystick.Horizontal, moveJoystick.Vertical).magnitude;

        if (joystickMagnitude < movementThreshold)
        {
            SetState("Idle");
        }
        else if (joystickMagnitude >= movementThreshold && joystickMagnitude < walkingThreshold)
        {
            SetState("StartWalking");
        }
        else
        {
            SetState("Walking");
        }
    }

    public void SetState(string newState)
    {
        if (newState == currentState) return;

        currentState = newState;

        switch (newState)
        {
            case "Idle":
                animator.SetTrigger("ToIdle");
                animator.SetBool("ToStartWalking", false);
                break;
            case "StartWalking":
                animator.SetBool("ToStartWalking", true);
                break;
            case "Walking":
                animator.SetBool("ToStartWalking", true);
                break;
            case "Collecting":
                animator.SetTrigger("ToCollecting");
                break;
        }
    }
}
