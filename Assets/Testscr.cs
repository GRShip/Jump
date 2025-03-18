using System;
using UnityEngine;

public class Testscr : MonoBehaviour {
    private Rigidbody rb;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if (rb.isKinematic == false) {
            Debug.Log(other.gameObject.name);
            if (transform.root != null) {
                PlayerPawn pawn = transform.root.GetComponent<PlayerPawn>();
                if (pawn != null) {
                    pawn.OnTriggerEnter(other);
                }
            }
        }
    }
}
