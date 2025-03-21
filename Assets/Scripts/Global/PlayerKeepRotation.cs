using UnityEngine;

public class PlayerKeepRotation : MonoBehaviour, IPawnComponent {
    public ThirdPersonPawn Owner { get; private set; }
    [HideInInspector]
    public Quaternion saveRotation;

    private void Awake() {
        saveRotation = transform.rotation;
    }

    private void Start() {
        Owner = GetComponentInParent<PlayerPawn>();
    }

    private void FixedUpdate() {
        LateUpdate();
    }
    
    private void LateUpdate() {
        transform.rotation = saveRotation;
    }
    
    public void DeActive() {
        enabled = true;
    }

    public void Active() {
        enabled = false;
    }
}
