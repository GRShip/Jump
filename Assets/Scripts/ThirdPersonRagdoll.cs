using System.Collections;
using UnityEngine;

public class ThirdPersonRagdoll : MonoBehaviour {
    private CharacterController controller;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private Animator animator;
    public GameObject hips;
    
    void Awake() {
        controller = GetComponentInParent<CharacterController>();
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        
        SetRagdollState(false);
    }
    
    public void SetRagdollState(bool state) {
        Vector3 velocity = Vector3.zero;
        if (controller != null) {
            controller.enabled = !state;
            velocity = controller.velocity;
        }
        
        if (animator != null) {
            animator.enabled = !state;
        }

        foreach (Rigidbody rb in ragdollBodies) {
            if (state == false) {
                rb.linearVelocity = velocity;
                rb.angularVelocity = Vector3.zero;
            }
            rb.isKinematic = !state;
        }

        foreach (Collider col in ragdollColliders) {
            col.enabled = state;
        }
        
        StartCoroutine(WaitPhysicsUpdate(velocity));
    }

    private IEnumerator WaitPhysicsUpdate(Vector3 velocity) {
        // FixedUpdate까지 대기
        yield return new WaitForFixedUpdate();
        //yield return new WaitForSeconds(0.02f);

        foreach (Rigidbody rb in ragdollBodies) {
            rb.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public Transform GetRagdollTransform() {
        return hips.transform;
    }
}
