using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ThirdPersonPawn))]
public class ThirdPersonInteract : MonoBehaviour, IPawnComponent {
    public ThirdPersonPawn Owner { get; private set; }
    
    [Min(0)] public float length = 5.0f;
    public LayerMask collisionLayer = -1;

    private void Start() {
        Owner = GetComponent<PlayerPawn>();
    }
    
    private void Raycasting() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, length, collisionLayer)) {
            Debug.DrawRay(transform.position, transform.forward * length, Color.green);
            Debug.Log(hit.transform.gameObject.name);
        }
    }

    public void DeActive() {
        enabled = false;
    }
    public void Active() {
        enabled = true;
    }

    public void OnInteract(InputValue value) {
        if (value.isPressed) {
            Raycasting();
        }
    }
}
