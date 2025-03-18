using System.Collections;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour, IPawnComponent {
    public ThirdPersonPawnController Controller { get; private set; }
    public bool Activity { get; private set; }
    
    private CharacterController cc;
    private Rigidbody[] rbs;
    private Collider[] cols;
    private Animator ani;
    public GameObject hip;
    
    private void Awake() {
        rbs = GetComponentsInChildren<Rigidbody>();
        cols = GetComponentsInChildren<Collider>();
        ani = GetComponent<Animator>();
    }

    private void Start() {
        cc = GetComponentInParent<CharacterController>();
        
        SetRagdollState(false);
        Controller = GetComponentInParent<PlayerPawn>().GetController();
    }

    public void SetRagdollState(bool state) {
        Activity = state;
        Vector3 velocity = Vector3.zero;
        if (cc) {
            cc.enabled = !state;
            velocity = cc.velocity;
        }
        
        if (ani) {
            ani.enabled = !state;
        }

        foreach (Rigidbody rb in rbs) {
            if (state == false) {
                rb.linearVelocity = velocity;
                rb.angularVelocity = Vector3.zero;
            }
            else {
                rb.AddForce(velocity, ForceMode.VelocityChange);
            }
            rb.isKinematic = !state;
        }

        foreach (Collider col in cols) {
            col.isTrigger = !state;
        }

        if (state == true) {
            //StartCoroutine(WaitPhysicsUpdate(velocity));
        }
    }

    private IEnumerator WaitPhysicsUpdate(Vector3 velocity) {
        // FixedUpdate까지 대기
        yield return new WaitForFixedUpdate();
        foreach (Rigidbody rb in rbs) {
            if (rb) {
                rb.AddForce(velocity, ForceMode.VelocityChange);
            }
        }
    }

    public GameObject GetRagdollRoot() {
        return hip;
    }
    
    public void DeActive() {
        SetRagdollState(true);
    }
    public void Active() {
        SetRagdollState(false);
    }
}
