using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour, IPawnComponent {
    private CharacterController ctrl;
    private Animator animator;
    private ThirdPersonInput input;
    
    [Header("플레이어")]
    [Tooltip("걷기속도")]
    public float moveSpeed = 2f;
    [Tooltip("달리기속도")]
    public float sprintSpeed = 6f;
    [Tooltip("가속도")]
    public float speedChangeRate = 10.0f;
    [Tooltip("회전보간시간")]
    [Range(0.0f, 0.3f)]
    public float rotationSmoothTime = 0.15f;
    private float hspeed = 0f;
    
    [Space(5)]
    [Tooltip("점프 세기")]
    public float jumpPower = 2f;
    [Tooltip("최대 점프 횟수"), Min(1)]
    public int jumpCountMax = 1;
    [FormerlySerializedAs("_jumpCount")]
    [SerializeField]
    private int jumpCount = 0;
    [Tooltip("점프 유지시간"), Range(0, 0.5f)]
    public float jumpHoldTime = 0.25f;
    private float jumpHold = 0;
    private bool jumpHoldFlag = false;
    [Tooltip("중력 세기")]
    public float gravityPower = -9.81f;
    private float vspeed = 0f;
    private const float vspeedMax = 53.0f;
    private float fallTime = 0f;
    private const float FallDelay = 0.15f;
    
    [Space(5)]
    [Header("땅")]
    [Tooltip("땅 위")]
    public bool onGround = true;
    [Tooltip("탐지 보정")]
    public float groundOffset = 0.1f;
    [Tooltip("탐지 레이어")]
    public LayerMask groundLayer = -1;
    
    [Space(5)]
    [Header("전방")]
    [Tooltip("전방위치")]
    public GameObject forwardPosition;
    [Tooltip("상승최대각도")]
    public float forwardPitchTop = 60.0f;
    [Tooltip("하강최대각도")]
    public float forwardPitchBottom = -60.0f;
    [Tooltip("회전감도")]
    public float forwardRotationRate = 10f;
    private float forwardYaw = 0f;
    private float forwardPitch = 0f;
    private float targetRotation = 0f;
    private float rotationVelocity = 0f;

    private int animIDSpeed;
    private int animIDLand;
    private int animIDJump;
    private int animIDFall;
    
    private void Awake() {
        ctrl = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        input = GetComponent<ThirdPersonInput>();
    }

    private void Start() {
        AssignAnimation();
    }

    private void Update() {
        //if (TryGetComponent(out _ctrl) == false) {
        if (ctrl.enabled == false) {
            return;
        }

        GroundedCheck();
        Gravity();
        Move();
        CameraRotation();
    }
    
    private void AssignAnimation() {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDLand = Animator.StringToHash("Land");
        animIDJump = Animator.StringToHash("Jump");
        animIDFall = Animator.StringToHash("Fall");
    }
    
    private void Gravity() {
        if (onGround == true) {
            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFall, false);
            
            fallTime = FallDelay;
            
            jumpCount = 0;
            jumpHold = 0;
            
            if (vspeed < 0.0f) {
                vspeed = -2f;  //-0.2보다 크면 ctrl.isGrounded값이 불안정해짐
            }
        }
        else {
            if (fallTime >= 0f) {
                fallTime -= Time.deltaTime;
            }
            else {
                animator.SetBool(animIDFall, true);
            }
        }
        
        if (input.jumpInput == true) {
            if (jumpCount < jumpCountMax) {
                if (jumpHoldFlag == false) {
                    jumpCount += 1;
                    jumpHold = 0;
                    jumpHoldFlag = true;
                    animator.SetBool(animIDJump, true);
                }
            }
        }
        else {
            jumpHoldFlag = false;
        }

        if (jumpHoldFlag == true) {
            vspeed = Mathf.Sqrt(jumpPower * -2f * gravityPower);
            jumpHold += Time.deltaTime;
            if (jumpHold > jumpHoldTime) {
                jumpHoldFlag = false;
            }
        }
        
        if (vspeed < vspeedMax) {
            vspeed += gravityPower * Time.deltaTime;
        }
    }

    private void GroundedCheck() {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        onGround = Physics.CheckSphere(spherePosition, ctrl.radius, 1 << groundLayer, QueryTriggerInteraction.Ignore);

        onGround = ctrl.isGrounded;
        animator.SetBool(animIDLand, onGround);
    }
    
    private void CameraRotation() {
        if (input.lookInput.sqrMagnitude >= 0.01f) {
            //마우스 이동
            forwardYaw += input.lookInput.x * Time.deltaTime * forwardRotationRate;
            forwardPitch += -input.lookInput.y * Time.deltaTime * forwardRotationRate;
        }
        
        //오버플로방지
        forwardYaw = Mathf.Clamp(forwardYaw, float.MinValue, float.MaxValue);
        forwardPitch = Mathf.Clamp(forwardPitch, forwardPitchBottom, forwardPitchTop);
        
        //회전
        forwardPosition.transform.rotation = Quaternion.Euler(forwardPitch, forwardYaw, 0.0f);
    }

    private void Move() {
        //목표 속도
        float targetSpeed = input.sprintInput ? sprintSpeed : moveSpeed;
        if (input.moveInput == Vector2.zero) {
            targetSpeed = 0.0f;
        }
        
        //현재 속도
        float currentSpeed = new Vector3(ctrl.velocity.x, 0.0f, ctrl.velocity.z).magnitude;
        if (Mathf.Abs(currentSpeed - targetSpeed) < 0.1f) {
            hspeed = targetSpeed;
        } else {
            hspeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        }
        
        Vector3 inputDirection = new Vector3(input.moveInput.x, 0.0f, input.moveInput.y).normalized;
        //입력 방향
        if (input.moveInput != Vector2.zero) {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + forwardPosition.transform.eulerAngles.y;
            
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        
        //이동
        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
        ctrl.Move((targetDirection.normalized * hspeed + new Vector3(0.0f, vspeed, 0.0f)) * Time.deltaTime);
        
        //애니메이터
        animator.SetFloat(animIDSpeed, hspeed);
    }
    
    
    
    public void OnFootstep(AnimationEvent animationEvent) {
        //if (animationEvent.animatorClipInfo.weight > 0.5f) {
            Debug.Log("StepSound");
        //}
    }

    public void OnLand(AnimationEvent animationEvent) {
        Debug.Log("LandSound");
    }
    
    public void DeActive() {
        enabled = false;
    }
    public void Active() {
        enabled = true;
    }
}
