using System;
using UnityEditor;
using UnityEngine;
using Vector3 = System.Numerics.Vector3;

public class SavePoint : MonoBehaviour {
    public int index = 0;

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.name);
        switch (other.gameObject.tag) {
        case "Player":
            if (GameManager.Instance.GetSaveIndex() <= index) {
                PlayerPawn pawn = other.gameObject.GetComponent<PlayerPawn>();
                if ((pawn) && (pawn.GetController())) {
                    Debug.Log("Call Save");
                    GameManager.Instance.SavePosition(gameObject.transform, index);
                }
            }
            break;
        }
    }
    
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Handles.Label(transform.position, $"index: {index}");
    }
}
