using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour {
    private InputActionMap _actionMap;
    private InputAction _actionMove;
    private CharacterController ctrl;

    private Transform springArm;
    private Vector3 velocity = Vector3.zero;
    
    private Vector2 moveInput = Vector2.zero;
    [Tooltip("기본속도"), Min(0)]
    public float moveSpeed = 5.0f;
    [Tooltip("달리기속도"), Min(0)]
    public float sprintSpeed = 8.0f;
    private bool sprintInput = false;

    private Vector3 jumpInput = Vector3.zero;
    [Tooltip("점프 힘")]
    public float jumpForce = 3.0f;
    [Tooltip("최대 점프 횟수"), Min(1)]
    public int jumpCountMax = 1;
    private int jumpCount = 0;
    [Tooltip("입력에 따른 점프높이 변화"), Range(0, 0.5f)]
    public float jumpHoldTime = 0.3f;
    private float jumpHold = 0;
    private bool jumpHoldFlag = false;
    [Tooltip("중력 값")]
    public float gravity = -9.81f;
    
    private Vector2 mouseInput = Vector2.zero;
    [Tooltip("마우스 감도")]
    public float mouseSensitivity = 10f;
    private float mouseYaw = 0f;
    private float mousePitch = 0f;
    public Vector2 mousePitchClamp = new Vector2(-60f, 60f);
    
    private void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        ctrl = GetComponent<CharacterController>();
        _actionMap = GetComponent<PlayerInput>().actions.FindActionMap("Player");
        _actionMap.FindAction("Sprint").performed += ctx => { sprintInput = true; };
        _actionMap.FindAction("Sprint").canceled += ctx => { sprintInput = false; };
        _actionMap.FindAction("Jump").performed += JumpHoldStart;
        _actionMap.FindAction("Jump").canceled += JumpHoldEnd;
        
        springArm = transform.Find("SpringArm");
    }

    private void FixedUpdate() {
        //중력
        velocity.y += gravity * Time.deltaTime;
        if ((ctrl.isGrounded == true) && (velocity.y < 0)) {
            jumpCount = 0;
            velocity.y = -0.2f; //-0.2보다 크면 ctrl.isGrounded값이 불안정해짐
        }
    }

    private void Update() {
        //Debug.Log(jumpHoldFlag.ToString()+"  "+jumpHold.ToString()+"  "+jumpCount.ToString()+"/"+jumpCountMax.ToString());
        //점프
        if ((jumpHoldFlag == true) && (jumpHold < jumpHoldTime) && (jumpCount < jumpCountMax)) {
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpForce);
            jumpHold += Time.deltaTime;
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
    }
    
    public void OnMove(InputValue input) {
        moveInput = input.Get<Vector2>();
    }

    //public void OnSprint(InputValue input) {
    //    sprintInput = input.Get<float>() > 0;
    //}
    
    public void OnJump(InputValue input) {
        if (jumpCount < jumpCountMax) {
            jumpHold = 0;
            velocity.y = Mathf.Sqrt(-2f * gravity * jumpForce);
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
}
