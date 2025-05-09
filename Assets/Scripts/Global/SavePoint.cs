using UnityEditor;
using UnityEngine;

public class SavePoint : MonoBehaviour {
    public int index = 0;
    private BoxCollider box;
    
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
    
    //private void OnDrawGizmos() {
    //    box = GetComponent<BoxCollider>();
    //    Gizmos.color = Color.red;
    //    Matrix4x4 oldMatrix = Gizmos.matrix;
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.DrawWireCube(box.center, box.size);
    //    Gizmos.matrix = oldMatrix;
    //    Handles.Label(transform.position, $"index: {index}");
    //}
}
