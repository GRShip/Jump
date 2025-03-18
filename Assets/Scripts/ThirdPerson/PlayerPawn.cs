using System.Collections;
using UnityEngine;

public class PlayerPawn : ThirdPersonPawn {
    private PlayerRagdoll ragdoll;
    private Animator ani;
    public GameObject cameraPosition;
    
    private int animIDStandFront;
    private int animIDStandBack;
    
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
        if (Input.GetMouseButtonDown(1) && controller) {
            PlayerDamaged(Vector3.back * 100, 5f);
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
        ((PlayerPawnController)controller).canInput = true;
    }

    protected override void UnPossess() {
        ((PlayerPawnController)controller).canInput = false;
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
        
        yield return new WaitForSeconds(time);
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
        Vector3 hipForward = ragdoll.hip.transform.forward;
        hipForward.y = 0f;
        if (hipForward.sqrMagnitude > 0.001f) {
            hipForward.Normalize();

            bool isFacingUp = Vector3.Dot(hipForward, Vector3.forward) > 0;
            Debug.Log("A instance는 xz 평면 상에서 " + (isFacingUp ? "위" : "아래") + "를 바라봅니다.");
            if (isFacingUp == true) {
                ani.SetBool(animIDStandFront, true);
            }
            else {
                ani.SetBool(animIDStandBack, true);
            }
        }
        else {
            ani.SetBool(animIDStandBack, true);
        }
        
        ChangePawnActivity(true);
        yield return new WaitForSeconds(5f);
        ((PlayerPawnController)controller).canInput = true;
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
