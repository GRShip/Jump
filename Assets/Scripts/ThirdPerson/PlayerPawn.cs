using UnityEngine;

public class PlayerPawn : ThirdPersonPawn {
    private PawnRagdoll ragdoll;
    private PlayerMovement movement;
    public PlayerPawnController PlayerController { get; private set; }
    public GameObject cameraPosition;
    
    private PlayerState playerState = PlayerState.Idle;
    
    private Vector3 cameraPositionOffset;
    
    protected override void Awake() {
        base.Awake();
        
        movement = GetComponent<PlayerMovement>();
        ragdoll = transform.Find("Mannequin").Find("Clown").Find("Stickman_9").GetComponent<PawnRagdoll>();
        cameraPositionOffset = cameraPosition.transform.localPosition;
    }
    
    private void Update() {
        //Debug
        if (Input.GetMouseButtonDown(1) && movement.canInput == true) {
            PlayerRagdollStart(Vector3.forward * 200);
            //PlayerDestroy();
        }
    }
    
    private void OnDestroy() {
        if (controller) {
            controller.DetachFromPawn();
        }
    }

    public override void Possess(ThirdPersonPawnController ctrl) {
        controller = ctrl;
        PlayerController = ctrl as PlayerPawnController;
        movement.canInput = true;
        PawnComponentsActivity(true);
    }

    public override void UnPossess() {
        movement.canInput = false;
        PawnComponentsActivity(false);
    }
    
    public void PlayerDestroy() {
        if (controller) {
            controller.DetachFromPawn();
        }
    }

    public void PlayerRagdollStart(Vector3 force) {
        playerState = PlayerState.Ragdoll;
        movement.canInput = false;
        PawnComponentsActivity(false);
        ragdoll.StartRagdoll(force);
        cameraPosition.transform.parent = ragdoll.hip.transform;
        cameraPosition.transform.position = ragdoll.hip.transform.position + Vector3.up;
        Camera.main.transform.parent.GetComponent<CameraMovement>().ChangeTarget(cameraPosition, false);
    }

    public void PlayerRagdollFinish() {
        playerState = PlayerState.Standup;
        PawnComponentsActivity(true);
        cameraPosition.transform.parent = this.gameObject.transform;
        cameraPosition.transform.localPosition = cameraPositionOffset;
        Camera.main.transform.parent.GetComponent<CameraMovement>().ChangeTarget(cameraPosition, true);
    }

    public void PlayerStandUpFinish() {
        playerState = PlayerState.Idle;
        movement.canInput = true;
    }
    
    public void OnTriggerEnter(Collider other) {
        string str = other.gameObject.tag;
        switch (str) {
        case "OutofBound":
            Destroy(gameObject);
            break;
        case "DeathArea":
            PlayerDestroy();
            break;
        }
    }
}
