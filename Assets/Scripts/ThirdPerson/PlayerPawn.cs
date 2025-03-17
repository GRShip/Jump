using System.Collections;
using UnityEngine;

public class PlayerPawn : ThirdPersonPawn {
    private PlayerRagdoll ragdoll;
    private Animator ani;
    public PlayerSpringArm cameraPosition;
    
    private int animIDStandFront;
    private int animIDStandBack;
    
    protected override void Awake() {
        base.Awake();
        
        ani = GetComponentInChildren<Animator>();
        ragdoll = transform.Find("Mannequin").GetComponent<PlayerRagdoll>();
    }

    protected override void Start() {
        base.Start();
        animIDStandFront = Animator.StringToHash("StandFront");
        animIDStandBack = Animator.StringToHash("StandBack");
    }

    private void Update() {
        ani.SetBool(animIDStandFront, false);
        ani.SetBool(animIDStandBack, false);
        //Debug
        if (Input.GetMouseButtonDown(0) && controller) {
            PlayerDamaged(Vector3.zero, 5f);
        }
        
        if (Input.GetMouseButtonDown(1) && controller) {
            PlayerDestroy();
        }
    }

    protected override void Possess(ThirdPersonPawnController ctrl) {
        controller = ctrl;
    }

    protected override void UnPossess() {
        ChangePawnActivity(false);
    }
    
    public void PlayerDestroy() {
        UnPossessController();
    }
    
    public void PlayerDamaged(Vector3 force, float time) {
        UnPossess();
        cameraPosition.SetRagdoll(ragdoll.root);
        
        StopAllCoroutines();
        StartCoroutine(ResetRagdoll(time));
    }

    public IEnumerator ResetRagdoll(float time) {
        yield return new WaitForSeconds(time);
        Ray ray = new Ray(ragdoll.GetRagdollRoot().transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.SphereCast(ray, Mathf.Max(0.01f, 0.25f), out hit, 1f, 1 << LayerMask.NameToLayer("Terrain"))) {
            transform.position = hit.point;
        }
        else {
            transform.position = ragdoll.GetRagdollRoot().transform.position;
        }
        
        ani.SetBool(animIDStandBack, true);
        
        cameraPosition.ResetRagdoll();
        ragdoll.transform.localPosition = Vector3.zero;
        ChangePawnActivity(true);
    }
    
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("SavePoint")) {
            if (controller) {
                Debug.Log("SavePoint");
                GameManager.Instance.SavePosition(other.gameObject.transform);
            }
        }
    }
}
