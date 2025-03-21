using System.Collections;
using UnityEngine;

public class PlayerPawn : ThirdPersonPawn {
    public PlayerPawnController PlayerController { get; private set; }
    private PlayerRagdoll ragdoll;
    private Animator ani;
    public GameObject cameraPosition;
    
    private int animIDStandFront;
    private int animIDStandBack;
    
    private PlayerState playerState = PlayerState.Idle;
    
    protected override void Awake() {
        base.Awake();
        
        ani = GetComponentInChildren<Animator>();
        animIDStandFront = Animator.StringToHash("StandFront");
        animIDStandBack = Animator.StringToHash("StandBack");
    }

    protected override void Start() {
        base.Start();
        ragdoll = transform.Find("Mannequin").GetComponent<PlayerRagdoll>();
    }

    private void Update() {
        ani.SetBool(animIDStandFront, false);
        ani.SetBool(animIDStandBack, false);
        //Debug
        if (Input.GetMouseButtonDown(1) && PlayerController.canInput) {
            PlayerDamaged(Vector3.back * 10, 5f);
            //PlayerDestroy();
        }
        
        if (ragdoll.Activity == true) {
            cameraPosition.transform.position = ragdoll.hip.transform.position + new Vector3(0, 1.1f, 0);
        }
        else {
            cameraPosition.transform.localPosition = new Vector3(0, 2, 0);
        }
    }

    private void OnDestroy() {
        UnPossessController();
    }

    protected override void Possess(ThirdPersonPawnController ctrl) {
        controller = ctrl;
        PlayerController = controller as PlayerPawnController;
        PlayerController.canInput = true;
    }

    protected override void UnPossess() {
        PlayerController.canInput = false;
        ChangePawnActivity(false);
    }
    
    public void PlayerDestroy() {
        UnPossessController();
    }
    
    public void PlayerDamaged(Vector3 force, float time) {
        UnPossess();
        
        StopAllCoroutines();
        StartCoroutine(DoRagdoll(force, time));
    }

    public IEnumerator DoRagdoll(Vector3 force, float time) {
        yield return new WaitForFixedUpdate();
        
        Rigidbody rb = ragdoll.hip.GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.VelocityChange);
        Camera.main.transform.parent.GetComponent<CameraMovement>().ChangeTarget(cameraPosition);
        yield return new WaitForSeconds(time);
        
        Camera.main.transform.parent.GetComponent<CameraMovement>().ChangeTarget(cameraPosition);
        Ray ray = new Ray(ragdoll.GetRagdollRoot().transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.SphereCast(ray, 0.25f, out hit, 2f, LayerMask.NameToLayer("Terrain"))) {
            transform.position = hit.point;
        }
        else {
            transform.position = ragdoll.GetRagdollRoot().transform.position;
        }

        Vector3 hipUp = -ragdoll.hip.transform.right;
        hipUp.y = 0f;
        if (hipUp.sqrMagnitude > 0.001f) {
            hipUp.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(hipUp);
            transform.rotation = targetRotation;
        }
        
        bool hipForward = ragdoll.hip.transform.forward.y > 0;
        Debug.Log("hipForward "+(hipForward ? "위" : "아래"));
        if (hipForward == true) {
            ani.SetBool(animIDStandFront, true);
            transform.Rotate(0, 180, 0);
        }
        else {
            ani.SetBool(animIDStandBack, true);
        }

        ChangePawnActivity(true);
        yield return new WaitForSeconds(5f);
        PlayerController.canInput = true;
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
