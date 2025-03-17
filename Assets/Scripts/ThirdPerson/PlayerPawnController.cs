using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerPawnController : ThirdPersonPawnController {
    public static PlayerPawnController Instance;
    
    [Header("입력")]
    [Tooltip("이동 입력값")]
    public Vector2 moveInput;
    [Tooltip("마우스 입력값")]
    public Vector2 lookInput;
    [Tooltip("점프 입력값")]
    public bool jumpInput;
    [Tooltip("달리기 입력값")]
    public bool sprintInput;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
    }
    
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
    
    protected override void AttachPawn(ThirdPersonPawn pawn) {
        PlayerPawn player = pawn as PlayerPawn;
        Transform target = player.cameraPosition._arm;
        Camera.main.GetComponent<CameraMovement>().ChangeTarget(target);
        Transform tf = GameManager.Instance.LoadPosition();
        pawn.gameObject.transform.position = tf.position;
        pawn.gameObject.transform.rotation = tf.rotation;
    }

    protected override void DetachPawn() {
        CreatePawn();
    }
}
