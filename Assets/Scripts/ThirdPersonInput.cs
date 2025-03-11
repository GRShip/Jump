using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonInput : MonoBehaviour {
    [Header("입력")]
    [Tooltip("이동 입력값")]
    public Vector2 moveInput;
    [Tooltip("마우스 입력값")]
    public Vector2 lookInput;
    [Tooltip("점프 입력값")]
    public bool jumpInput;
    [Tooltip("달리기 입력값")]
    public bool sprintInput;

    [Header("입력 설정")]
    [Tooltip("아날로그")]
    public bool analogMovement;
    [Tooltip("마우스잠금")]
    public bool cursorLocked = true;
    
    public void OnMove(InputValue value) {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value) {
        LookInput(value.Get<Vector2>());
    }

    public void OnJump(InputValue value) {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value) {
        SprintInput(value.isPressed);
    }
    
    public void MoveInput(Vector2 newMove) {
        moveInput = newMove;
    }

    public void LookInput(Vector2 newLook) {
        lookInput = newLook;
    }

    public void JumpInput(bool newJump) {
        jumpInput = newJump;
    }

    public void SprintInput(bool newSprint) {
        sprintInput = newSprint;
    }

    private void OnApplicationFocus(bool hasFocus) {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState) {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
