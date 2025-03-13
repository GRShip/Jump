using System;
using UnityEngine;

public class SavePoint : MonoBehaviour {
    private BoxCollider boxCollider;
    private void Awake() {
        boxCollider = GetComponent<BoxCollider>();
    }
}
