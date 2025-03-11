using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour {
    private CharacterController _ctrl;
    private Animator _animator;
    private ThirdPersonInput _input;
    
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
    private float _hspeed = 0f;
    
    [Space(5)]
    [Tooltip("점프 세기")]
    public float jumpPower = 2f;
    [Tooltip("최대 점프 횟수"), Min(1)]
    public int jumpCountMax = 1;
    [SerializeField]
    private int _jumpCount = 0;
    [Tooltip("점프 유지시간"), Range(0, 0.5f)]
    public float jumpHoldTime = 0.25f;
    private float _jumpHold = 0;
    private bool _jumpHoldFlag = false;
    [Tooltip("중력 세기")]
    public float gravityPower = -9.81f;
    private float _fallDelay = 0.15f;
    private float _vspeed = 0f;
    private float _vspeedMax = 53.0f;
    private float _fallTime = 0f;
    
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
    private float _forwardYaw = 0f;
    private float _forwardPitch = 0f;
    private float _targetRotation = 0f;
    private float _rotationVelocity = 0f;

    private int _animIDSpeed;
    private int _animIDLand;
    private int _animIDJump;
    private int _animIDFall;
    
    private void Awake() {
        _ctrl = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        _input = GetComponent<ThirdPersonInput>();
    }

    private void Start() {
        AssignAnimation();
    }

    private void Update() {
        //if (TryGetComponent(out _ctrl) == false) {
        if (_ctrl.enabled == false) {
            return;
        }

        GroundedCheck();
        Gravity();
        Move();
        CameraRotation();
    }
    
    private void AssignAnimation() {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDLand = Animator.StringToHash("Land");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFall = Animator.StringToHash("Fall");
    }
    
    private void Gravity() {
        if (onGround == true) {
            _animator.SetBool(_animIDJump, false);
            _animator.SetBool(_animIDFall, false);
            
            _fallTime = _fallDelay;
            
            _jumpCount = 0;
            _jumpHold = 0;
            
            if (_vspeed < 0.0f) {
                _vspeed = -2f;  //-0.2보다 크면 ctrl.isGrounded값이 불안정해짐
            }
        }
        else {
            if (_fallTime >= 0f) {
                _fallTime -= Time.deltaTime;
            }
            else {
                _animator.SetBool(_animIDFall, true);
            }
        }
        
        if (_input.jumpInput == true) {
            if (_jumpCount < jumpCountMax) {
                if (_jumpHoldFlag == false) {
                    _jumpCount += 1;
                    _jumpHold = 0;
                    _jumpHoldFlag = true;
                    _animator.SetBool(_animIDJump, true);
                }
            }
        }
        else {
            _jumpHoldFlag = false;
        }

        if (_jumpHoldFlag == true) {
            _vspeed = Mathf.Sqrt(jumpPower * -2f * gravityPower);
            _jumpHold += Time.deltaTime;
            if (_jumpHold > jumpHoldTime) {
                _jumpHoldFlag = false;
            }
        }
        
        if (_vspeed < _vspeedMax) {
            _vspeed += gravityPower * Time.deltaTime;
        }
    }

    private void GroundedCheck() {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        onGround = Physics.CheckSphere(spherePosition, _ctrl.radius, groundLayer, QueryTriggerInteraction.Ignore);

        onGround = _ctrl.isGrounded;
        _animator.SetBool(_animIDLand, onGround);
    }
    
    private void CameraRotation() {
        if (_input.lookInput.sqrMagnitude >= 0.01f) {
            //마우스 이동
            _forwardYaw += _input.lookInput.x * Time.deltaTime * forwardRotationRate;
            _forwardPitch += -_input.lookInput.y * Time.deltaTime * forwardRotationRate;
        }
        
        //오버플로방지
        _forwardYaw = Mathf.Clamp(_forwardYaw, float.MinValue, float.MaxValue);
        _forwardPitch = Mathf.Clamp(_forwardPitch, forwardPitchBottom, forwardPitchTop);
        
        //회전
        forwardPosition.transform.rotation = Quaternion.Euler(_forwardPitch, _forwardYaw, 0.0f);
    }

    private void Move() {
        //목표 속도
        float targetSpeed = _input.sprintInput ? sprintSpeed : moveSpeed;
        if (_input.moveInput == Vector2.zero) {
            targetSpeed = 0.0f;
        }
        
        //현재 속도
        float currentSpeed = new Vector3(_ctrl.velocity.x, 0.0f, _ctrl.velocity.z).magnitude;
        if (Mathf.Abs(currentSpeed - targetSpeed) < 0.1f) {
            _hspeed = targetSpeed;
        } else {
            _hspeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * speedChangeRate);
        }
        
        Vector3 inputDirection = new Vector3(_input.moveInput.x, 0.0f, _input.moveInput.y).normalized;
        //입력 방향
        if (_input.moveInput != Vector2.zero) {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + forwardPosition.transform.eulerAngles.y;
            
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, rotationSmoothTime);
            
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        
        //이동
        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
        _ctrl.Move((targetDirection.normalized * _hspeed + new Vector3(0.0f, _vspeed, 0.0f)) * Time.deltaTime);
        
        //애니메이터
        _animator.SetFloat(_animIDSpeed, _hspeed);
    }
    
    
    
    public void OnFootstep(AnimationEvent animationEvent) {
        //if (animationEvent.animatorClipInfo.weight > 0.5f) {
            Debug.Log("StepSound");
        //}
    }

    public void OnLand(AnimationEvent animationEvent) {
        Debug.Log("LandSound");
    }
}
