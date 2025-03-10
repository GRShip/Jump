using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonMovement : MonoBehaviour {
    [Header("플레이어")]
    [Tooltip("걷기속도")]
    public float moveSpeed = 2f;
    [Tooltip("달리기속도")]
    public float sprintSpeed = 6f;
    
    [Tooltip("")]
    public bool LockCameraPosition = false;
    
    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDLand;
    private int _animIDJump;
    private int _animIDFall;
    private int _animIDMotionSpeed;

    private CharacterController ctrl;
    private Animator animator;
    private bool hasAnimator;
    private PlayerInput playerInput;
    private ThirdPersonInput input;
    
    
    private GameObject mainCamera;
    private GameObject CameraPosition;
    private float CameraPositionYaw;
    private float CameraPositionPitch;
    
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -30.0f;

    private bool IsMouse {
        get { return playerInput.currentControlScheme == "KeyboardMouse"; }
    }

    private void Awake() {
        ctrl = GetComponent<CharacterController>();
        hasAnimator = TryGetComponent(out animator);
        input = GetComponent<ThirdPersonInput>();
        playerInput = GetComponent<PlayerInput>();
        
        if (mainCamera == null) {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start() {
        AssignAnimation();
    }

    private void Update() {
        hasAnimator = TryGetComponent(out animator);

        //JumpAndGravity();
        //GroundedCheck();
        //Move();
    }

    private void LateUpdate() {
        //CameraRotation();
    }

    private void AssignAnimation() {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDLand = Animator.StringToHash("Land");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFall = Animator.StringToHash("Fall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void CameraRotation() {
        if ((input.lookInput.sqrMagnitude >= 0.01f) && (!LockCameraPosition)) {
            float deltaTime = IsMouse ? 1.0f : Time.deltaTime;

            CameraPositionYaw += input.lookInput.x * deltaTime;
            CameraPositionPitch += input.lookInput.y * deltaTime;
        }
        
        CameraPositionYaw = Mathf.Clamp(CameraPositionYaw, float.MinValue, float.MaxValue);
        CameraPositionPitch = Mathf.Clamp(CameraPositionPitch, BottomClamp, TopClamp);
        
        CameraPosition.transform.rotation = Quaternion.Euler(CameraPositionPitch, CameraPositionYaw, 0.0f);
    }
}
