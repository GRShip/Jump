using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    private InputActionMap _actionMap;
    private CharacterController ctrl;
    private Animator _animator;

    private Transform springArm;
    private Vector3 velocity = Vector3.zero;

    private Vector2 moveInput = Vector2.zero;
    [Tooltip("기본속도"), Min(0)] public float moveSpeed = 2.0f;
    [Tooltip("달리기속도"), Min(0)] public float sprintSpeed = 6.0f;
    private bool sprintInput = false;

    private Vector3 jumpInput = Vector3.zero;
    [Tooltip("점프 힘")] public float jumpForce = 3.0f;
    [Tooltip("최대 점프 횟수"), Min(1)] public int jumpCountMax = 1;
    private int jumpCount = 0;

    [Tooltip("입력에 따른 점프높이 변화"), Range(0, 0.5f)]
    public float jumpHoldTime = 0.3f;

    private float jumpHold = 0;
    private bool jumpHoldFlag = false;
    [Tooltip("중력 값")] public float gravity = -9.81f;

    private Vector2 mouseInput = Vector2.zero;
    [Tooltip("마우스 감도")] public float mouseSensitivity = 10f;
    private float mouseYaw = 0f;
    private float mousePitch = 0f;
    public Vector2 mousePitchClamp = new Vector2(-60f, 60f);

    //Animation ID
    private int _animIDSpeed;
    private int _animIDLand;
    private int _animIDJump;
    private int _animIDFall;

    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start() {
        ctrl = GetComponent<CharacterController>();
        _actionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _actionMap.FindAction("Sprint").performed += ctx => { sprintInput = true; };
        _actionMap.FindAction("Sprint").canceled += ctx => { sprintInput = false; };
        _actionMap.FindAction("Jump").performed += JumpHoldStart;
        _actionMap.FindAction("Jump").canceled += JumpHoldEnd;

        springArm = transform.Find("SpringArm");

        _animator = transform.Find("Mannequin").GetComponent<Animator>();
        //bool c = TryGetComponent(out _animator);
        AssignAnimationIDs();
    }

    private void AssignAnimationIDs() {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDLand = Animator.StringToHash("Land");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFall = Animator.StringToHash("Fall");
    }

    private void FixedUpdate() {
        //중력
        _animator.SetBool(_animIDLand, false);
        velocity.y += gravity * Time.deltaTime;
        if ((ctrl.isGrounded == true) && (velocity.y < 0)) {
            jumpCount = 0;
            velocity.y = -0.2f; //-0.2보다 크면 ctrl.isGrounded값이 불안정해짐
            _animator.SetBool(_animIDLand, true);
        }
    }

    private void Update() {
        _animator.SetBool(_animIDJump, false);
        _animator.SetBool(_animIDFall, false);

        //점프
        if ((jumpHoldFlag == true) && (jumpHold < jumpHoldTime) && (jumpCount < jumpCountMax)) {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpForce);
            jumpHold += Time.deltaTime;
            _animator.SetBool(_animIDFall, true);
        }

        //마우스 회전
        mouseYaw += mouseInput.x * mouseSensitivity * Time.deltaTime;
        mousePitch += -mouseInput.y * mouseSensitivity * Time.deltaTime;
        mousePitch = Mathf.Clamp(mousePitch, mousePitchClamp.x, mousePitchClamp.y);
        transform.rotation = Quaternion.Euler(0f, mouseYaw, 0f);
        springArm.localRotation = Quaternion.Euler(mousePitch, 0f, 0f);

        //이동
        float speed = sprintInput == true ? sprintSpeed : moveSpeed;
        Vector3 move = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y) * speed);

        //종합
        ctrl.Move((velocity + move + jumpInput) * Time.deltaTime);
        _animator.SetFloat(_animIDSpeed, moveInput.magnitude > 0 ? speed : 0);
    }

    public void OnMove(InputValue input) {
        moveInput = input.Get<Vector2>();
    }

    public void OnJump(InputValue input) {
        if (jumpCount < jumpCountMax) {
            jumpHold = 0;
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpForce);
            _animator.SetBool(_animIDJump, true);
        }
    }

    public void OnLook(InputValue input) {
        mouseInput = input.Get<Vector2>();
    }

    public void JumpHoldStart(InputAction.CallbackContext ctx) {
        jumpHoldFlag = true;
    }

    public void JumpHoldEnd(InputAction.CallbackContext ctx) {
        jumpHoldFlag = false;
        jumpCount += 1;
    }

    public void OnFootstep(AnimationEvent animationEvent) {
        if (animationEvent.animatorClipInfo.weight > 0.5f) {
            Debug.Log("StepSound");
        }
    }

    public void OnLand(AnimationEvent animationEvent) {
        if (animationEvent.animatorClipInfo.weight > 0.5f) {
            Debug.Log("LandSound");
        }
    }
}
