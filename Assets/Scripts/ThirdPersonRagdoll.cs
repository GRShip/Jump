using System.Collections;
using UnityEngine;

public class ThirdPersonRagdoll : MonoBehaviour {
    public CharacterController controller;
    public Rigidbody[] ragdollBodies;
    public Collider[] ragdollColliders;
    public Animator animator;
    
    void Awake() {
        controller = GetComponentInParent<CharacterController>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        
        SetRagdollState(false);
    }
    
    public void SetRagdollState(bool state) {
        Debug.Log(gameObject.name+" RagdollState: " + state);
        Vector3 velocity = Vector3.zero;
        if (controller != null) {
            controller.enabled = !state;
            velocity = controller.velocity;
        }
        
        if (animator != null) {
            animator.enabled = !state;
        }
        
        foreach (Rigidbody rb in ragdollBodies) {
            rb.isKinematic = !state;
            rb.linearVelocity = velocity;
            rb.angularVelocity = Vector3.zero;
        }

        foreach (Collider col in ragdollColliders) {
            col.enabled = state;
        }
        
        StartCoroutine(WaitPhysicsUpdate(velocity));
    }

    private IEnumerator WaitPhysicsUpdate(Vector3 velocity) {
        // FixedUpdate까지 대기
        yield return new WaitForFixedUpdate();

        foreach (Rigidbody rb in ragdollBodies) {
            rb.linearVelocity = velocity;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
