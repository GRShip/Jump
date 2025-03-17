using System;
using System.Collections;
using UnityEngine;

public class PlayerRagdoll : MonoBehaviour, IPawnComponent {
    public ThirdPersonPawnController Controller { get; private set; }
    
    private CharacterController cc;
    private Rigidbody[] rb;
    private Collider[] col;
    private Animator ani;
    public GameObject root;
    
    private void Awake() {
        cc = GetComponentInParent<CharacterController>();
        rb = GetComponentsInChildren<Rigidbody>();
        col = GetComponentsInChildren<Collider>();
        ani = GetComponent<Animator>();
        
        SetRagdollState(false);
    }

    private void Start() {
        Controller = GetComponentInParent<PlayerPawn>().GetController();
    }

    public void SetRagdollState(bool state) {
        Vector3 velocity = Vector3.zero;
        if (cc) {
            cc.enabled = !state;
            velocity = cc.velocity;
        }
        
        if (ani) {
            ani.enabled = !state;
        }

        foreach (Rigidbody rb in rb) {
            if (state == false) {
                rb.linearVelocity = velocity;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = !state;
        }

        foreach (Collider col in col) {
            col.enabled = state;
        }

        if (state == true) {
            StartCoroutine(WaitPhysicsUpdate(velocity));
        }
    }

    private IEnumerator WaitPhysicsUpdate(Vector3 velocity) {
        // FixedUpdate까지 대기
        yield return new WaitForFixedUpdate();
        foreach (Rigidbody rb in rb) {
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public GameObject GetRagdollRoot() {
        return root;
    }
    
    public void DeActive() {
        SetRagdollState(true);
    }
    public void Active() {
        SetRagdollState(false);
    }
}
